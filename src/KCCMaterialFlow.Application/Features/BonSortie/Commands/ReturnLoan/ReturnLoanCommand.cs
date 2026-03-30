using FluentValidation;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.ReturnLoan;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record ReturnLoanCommand(
    int PretId,
    string ReceptionnePar,
    string? EtatRetour = null) : IRequest<Result>;

// ── Validator ───────────────────────────────────────────────────────────
public sealed class ReturnLoanCommandValidator : AbstractValidator<ReturnLoanCommand>
{
    public ReturnLoanCommandValidator()
    {
        RuleFor(x => x.PretId).GreaterThan(0);
        RuleFor(x => x.ReceptionnePar).NotEmpty().MaximumLength(200);
        RuleFor(x => x.EtatRetour).MaximumLength(1000).When(x => x.EtatRetour is not null);
    }
}

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class ReturnLoanCommandHandler(IApplicationDbContext dbContext)
    : IRequestHandler<ReturnLoanCommand, Result>
{
    public async Task<Result> Handle(ReturnLoanCommand request, CancellationToken ct)
    {
        var pret = await dbContext.Prets.FindAsync([request.PretId], ct);
        if (pret is null)
            return Result.Failure(Error.NotFound("Pret", request.PretId));

        var result = pret.EnregistrerRetour(request.ReceptionnePar, request.EtatRetour);
        if (result.IsFailure)
            return result;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
