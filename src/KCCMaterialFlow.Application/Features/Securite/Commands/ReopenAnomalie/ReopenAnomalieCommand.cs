using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Securite.Commands.ReopenAnomalie;

// ════════════════════════════════════════════════════════════════════
// 1 fichier = 1 use case complet : Command + Validator + Handler
// ════════════════════════════════════════════════════════════════════

public sealed record ReopenAnomalieCommand(
    int AnomalieId,
    string Motif) : IRequest<Result>;

public sealed class ReopenAnomalieCommandValidator : AbstractValidator<ReopenAnomalieCommand>
{
    public ReopenAnomalieCommandValidator()
    {
        RuleFor(x => x.AnomalieId).GreaterThan(0);
        RuleFor(x => x.Motif).NotEmpty().MaximumLength(2000);
    }
}

public sealed class ReopenAnomalieCommandHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<ReopenAnomalieCommand, Result>
{
    public async Task<Result> Handle(
        ReopenAnomalieCommand cmd, CancellationToken ct)
    {
        var anomalie = await dbContext.Anomalies
            .FirstOrDefaultAsync(a => a.Id == cmd.AnomalieId, ct);

        if (anomalie is null)
            return Result.Failure(Error.NotFound("Anomalie", cmd.AnomalieId));

        if (!anomalie.EstTraitee)
            return Result.Failure(Error.Validation(
                "Securite.AnomalieNonTraitee",
                $"L'anomalie #{cmd.AnomalieId} n'est pas encore traitée, elle ne peut pas être réouverte."));

        // Use reflection to reset private setters — the domain entity doesn't expose a Reopen method
        // so we use EF's change tracker to update the fields directly
        dbContext.Anomalies.Entry(anomalie).Property("EstTraitee").CurrentValue = false;
        dbContext.Anomalies.Entry(anomalie).Property("DateTraitement").CurrentValue = null;
        dbContext.Anomalies.Entry(anomalie).Property("TraitePar").CurrentValue = null;
        dbContext.Anomalies.Entry(anomalie).Property("Resolution").CurrentValue = $"[Réouvert] {cmd.Motif}";
        dbContext.Anomalies.Entry(anomalie).Property("ActionsCorrectives").CurrentValue = null;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
