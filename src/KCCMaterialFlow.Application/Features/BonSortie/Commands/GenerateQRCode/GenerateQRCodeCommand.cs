using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonSortie.Commands.GenerateQRCode;

// ── Command ─────────────────────────────────────────────────────────────
public sealed record GenerateBonSortieQRCodeCommand(int BonSortieId) : IRequest<Result<string>>;

// ── Handler ─────────────────────────────────────────────────────────────
internal sealed class GenerateBonSortieQRCodeCommandHandler(
    IApplicationDbContext dbContext,
    IQRCodeService qrCodeService)
    : IRequestHandler<GenerateBonSortieQRCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateBonSortieQRCodeCommand request, CancellationToken ct)
    {
        var bon = await dbContext.BonsSortie.FindAsync([request.BonSortieId], ct);
        if (bon is null)
            return Result.Failure<string>(Error.NotFound("BonSortie", request.BonSortieId));

        var qrData = $"BONSORTIE|{bon.Id}|{bon.NumeroReference}|{bon.StatutActuel}|{bon.DateCreation:yyyyMMdd}";
        var (_, base64, hash) = await qrCodeService.GenerateAsync(qrData, ct);

        bon.SetQRCode(qrData, base64, hash);
        await dbContext.SaveChangesAsync(ct);

        return base64;
    }
}
