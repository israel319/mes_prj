using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.ExtendLoan;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record ExtendLoanCommand(
    int PretId,
    DateTime NouvelleDateRetour) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class ExtendLoanCommandValidator : AbstractValidator<ExtendLoanCommand>
{
    public ExtendLoanCommandValidator()
    {
        RuleFor(x => x.PretId).GreaterThan(0);
        RuleFor(x => x.NouvelleDateRetour).GreaterThan(DateTime.Now);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class ExtendLoanCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<ExtendLoanCommand, Result>
{
    public async Task<Result> Handle(ExtendLoanCommand request, CancellationToken ct)
    {
        var pret = await dbContext.Prets.FindAsync([request.PretId], ct);
        if (pret is null)
            return Result.Failure(Error.NotFound("Pret", request.PretId));

        var result = pret.ProlongerPret(request.NouvelleDateRetour);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
