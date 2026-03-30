using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Securite.Commands.CreateAnomalie;

// ════════════════════════════════════════════════════════════════════
// 1 fichier = 1 use case complet : Command + Validator + Handler
// ════════════════════════════════════════════════════════════════════

public sealed record CreateAnomalieCommand(
    int? ScanId,
    int? BonId,
    string? TypeBon,
    int? BarriereId,
    string Type,
    string Description,
    string NiveauGravite) : IRequest<Result<int>>;

public sealed class CreateAnomalieCommandValidator : AbstractValidator<CreateAnomalieCommand>
{
    public CreateAnomalieCommandValidator()
    {
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.NiveauGravite).NotEmpty().MaximumLength(20);
    }
}

public sealed class CreateAnomalieCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<CreateAnomalieCommand, Result<int>>
{
    public async Task<Result<int>> Handle(
        CreateAnomalieCommand cmd, CancellationToken ct)
    {
        var signalePar = currentUser.GetUserLogin();
        var signaleParNom = currentUser.GetUserDisplayName();

        var anomalie = new Anomalie(
            typeAnomalie: cmd.Type,
            description: cmd.Description,
            signalePar: signalePar,
            niveauGravite: cmd.NiveauGravite,
            bonId: cmd.BonId,
            typeBon: cmd.TypeBon,
            scanId: cmd.ScanId,
            barriereId: cmd.BarriereId,
            signaleParNom: signaleParNom);

        // If linked to a scan, flag the scan as having an anomalie
        if (cmd.ScanId.HasValue)
        {
            var scan = await dbContext.ScansEvenement
                .FirstOrDefaultAsync(s => s.Id == cmd.ScanId.Value, ct);
            scan?.MarquerAnomalie();
        }

        dbContext.Anomalies.Add(anomalie);
        await dbContext.SaveChangesAsync(ct);

        return anomalie.Id;
    }
}
