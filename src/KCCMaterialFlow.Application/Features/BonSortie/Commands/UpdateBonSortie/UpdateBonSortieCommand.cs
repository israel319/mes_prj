using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.UpdateBonSortie;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record UpdateBonSortieCommand(
    int BonSortieId,
    string MotifSortie,
    string Provenance,
    string Destination,
    DateTime DateExpiration,
    string? Description = null) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class UpdateBonSortieCommandValidator : AbstractValidator<UpdateBonSortieCommand>
{
    public UpdateBonSortieCommandValidator()
    {
        RuleFor(x => x.BonSortieId).GreaterThan(0);
        RuleFor(x => x.MotifSortie).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Provenance).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Destination).NotEmpty().MaximumLength(200);
        RuleFor(x => x.DateExpiration).GreaterThan(DateTime.Now);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class UpdateBonSortieCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<UpdateBonSortieCommand, Result>
{
    public async Task<Result> Handle(UpdateBonSortieCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie.FindAsync([request.BonSortieId], ct);
        if (bon is null)
            return Result.Failure(Error.NotFound("BonSortie", request.BonSortieId));

        var result = bon.MettreAJour(
            request.MotifSortie, request.Provenance, request.Destination,
            request.DateExpiration, request.Description);

        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
