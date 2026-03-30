using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Errors;
using KCCMaterialFlow.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Securite.Commands.ProcessScan;

// ════════════════════════════════════════════════════════════════════
// 1 fichier = 1 use case complet : Command + DTO + Validator + Handler
// ════════════════════════════════════════════════════════════════════

public sealed record ProcessScanCommand(
    int BonId,
    string TypeBon,
    string QRCodeData,
    int BarriereId,
    string? AgentNom,
    string? Observations,
    string? AdresseIp,
    double? Latitude,
    double? Longitude) : IRequest<Result<ScanResultDto>>;

public sealed record ScanResultDto
{
    public int ScanId { get; init; }
    public bool EstValide { get; init; }
    public string Statut { get; init; } = string.Empty;
    public string? NumeroPreuve { get; init; }
    public string? MessageErreur { get; init; }
}

public sealed class ProcessScanCommandValidator : AbstractValidator<ProcessScanCommand>
{
    public ProcessScanCommandValidator()
    {
        RuleFor(x => x.BonId).GreaterThan(0);
        RuleFor(x => x.QRCodeData).NotEmpty();
        RuleFor(x => x.BarriereId).GreaterThan(0);
    }
}

public sealed class ProcessScanCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser,
    IQRCodeService qrCodeService)
    : IRequestHandler<ProcessScanCommand, Result<ScanResultDto>>
{
    public async Task<Result<ScanResultDto>> Handle(
        ProcessScanCommand cmd, CancellationToken ct)
    {
        var barriere = await dbContext.Barrieres
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == cmd.BarriereId, ct);

        if (barriere is null)
            return Result.Failure<ScanResultDto>(Error.NotFound("Barriere", cmd.BarriereId));

        var agentLogin = currentUser.GetUserLogin();
        var agentNom = cmd.AgentNom ?? currentUser.GetUserDisplayName();

        var scan = new ScanEvenement(
            barriereId: cmd.BarriereId,
            agentLogin: agentLogin,
            typeMouvement: cmd.TypeBon == "BE" ? TypeMouvementValues.Entree : TypeMouvementValues.Sortie,
            qrCodeData: cmd.QRCodeData,
            bonId: cmd.BonId,
            typeBon: cmd.TypeBon,
            agentNom: agentNom);

        if (!string.IsNullOrWhiteSpace(cmd.Observations))
            scan.SetObservations(cmd.Observations);

        // Validate QR code against expected hash on the bon
        string? expectedHash = await GetExpectedHash(cmd.BonId, cmd.TypeBon, ct);
        bool isValid = expectedHash is null
            || await qrCodeService.ValidateAsync(cmd.QRCodeData, expectedHash, ct);

        if (isValid)
        {
            scan.MarquerValide();
            var preuve = $"PRV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            scan.SetNumeroPreuve(preuve);
        }
        else
        {
            scan.MarquerInvalide("QR code invalide ou non conforme.");
        }

        dbContext.ScansEvenement.Add(scan);
        await dbContext.SaveChangesAsync(ct);

        scan.AddDomainEvent(new ScanProcessedEvent(scan.Id, cmd.BarriereId, cmd.QRCodeData, isValid));

        return Result.Success(new ScanResultDto
        {
            ScanId = scan.Id,
            EstValide = isValid,
            Statut = scan.StatutScan,
            NumeroPreuve = scan.NumeroPreuve,
            MessageErreur = isValid ? null : "QR code invalide ou non conforme."
        });
    }

    private async Task<string?> GetExpectedHash(int bonId, string typeBon, CancellationToken ct)
    {
        if (typeBon == "BE")
        {
            return await dbContext.BonsEntree
                .Where(b => b.Id == bonId)
                .Select(b => b.QRCodeHash)
                .FirstOrDefaultAsync(ct);
        }

        return await dbContext.BonsSortie
            .Where(b => b.Id == bonId)
            .Select(b => b.QRCodeHash)
            .FirstOrDefaultAsync(ct);
    }
}
