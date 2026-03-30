using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Securite.Commands.GenererPreuvePassage;

// ════════════════════════════════════════════════════════════════════
// 1 fichier = 1 use case complet : Command + DTO + Handler
// ════════════════════════════════════════════════════════════════════

public sealed record GenererPreuvePassageCommand(int ScanId) : IRequest<Result<PreuvePassageDto>>;

public sealed record PreuvePassageDto
{
    public int ScanId { get; init; }
    public string NumeroPreuve { get; init; } = string.Empty;
    public DateTime DateScan { get; init; }
    public string BarriereNom { get; init; } = string.Empty;
    public string AgentNom { get; init; } = string.Empty;
    public int? BonId { get; init; }
    public string? TypeBon { get; init; }
    public string ResultatScan { get; init; } = string.Empty;
}

public sealed class GenererPreuvePassageCommandHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<GenererPreuvePassageCommand, Result<PreuvePassageDto>>
{
    public async Task<Result<PreuvePassageDto>> Handle(
        GenererPreuvePassageCommand cmd, CancellationToken ct)
    {
        var scan = await dbContext.ScansEvenement
            .FirstOrDefaultAsync(s => s.Id == cmd.ScanId, ct);

        if (scan is null)
            return Result.Failure<PreuvePassageDto>(Error.NotFound("ScanEvenement", cmd.ScanId));

        // Generate preuve number if not yet assigned
        if (string.IsNullOrWhiteSpace(scan.NumeroPreuve))
        {
            var preuve = $"PRV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            scan.SetNumeroPreuve(preuve);
            await dbContext.SaveChangesAsync(ct);
        }

        var barriere = await dbContext.Barrieres
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == scan.BarriereId, ct);

        return Result.Success(new PreuvePassageDto
        {
            ScanId = scan.Id,
            NumeroPreuve = scan.NumeroPreuve!,
            DateScan = scan.DateHeureScan,
            BarriereNom = barriere?.NomBarriere ?? string.Empty,
            AgentNom = scan.AgentNom ?? string.Empty,
            BonId = scan.BonId,
            TypeBon = scan.TypeBon,
            ResultatScan = scan.StatutScan
        });
    }
}
