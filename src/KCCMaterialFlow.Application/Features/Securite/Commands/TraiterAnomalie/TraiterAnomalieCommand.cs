using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Securite.Commands.TraiterAnomalie;

// ════════════════════════════════════════════════════════════════════
// 1 fichier = 1 use case complet : Command + Validator + Handler
// ════════════════════════════════════════════════════════════════════

public sealed record TraiterAnomalieCommand(
    int AnomalieId,
    string Action,
    string? Resolution,
    string? ActionsCorrectives) : IRequest<Result>;

public sealed class TraiterAnomalieCommandValidator : AbstractValidator<TraiterAnomalieCommand>
{
    public TraiterAnomalieCommandValidator()
    {
        RuleFor(x => x.AnomalieId).GreaterThan(0);
        RuleFor(x => x.Action).NotEmpty().MaximumLength(2000);
    }
}

public sealed class TraiterAnomalieCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser)
    : IRequestHandler<TraiterAnomalieCommand, Result>
{
    public async Task<Result> Handle(
        TraiterAnomalieCommand cmd, CancellationToken ct)
    {
        var anomalie = await dbContext.Anomalies
            .FirstOrDefaultAsync(a => a.Id == cmd.AnomalieId, ct);

        if (anomalie is null)
            return Result.Failure(Error.NotFound("Anomalie", cmd.AnomalieId));

        // Domain method handles validation + domain event
        var resolution = cmd.Resolution ?? cmd.Action;
        var result = anomalie.Traiter(
            currentUser.GetUserLogin(),
            resolution,
            cmd.ActionsCorrectives);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
