using System.Text.Json;
using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;

namespace KCCMaterialFlow.Application.Features.BonEntree.Commands.GenerateQRCode;

public sealed record GenerateQRCodeCommand(int BonEntreeId) : IRequest<Result<string>>;

public sealed class GenerateQRCodeCommandHandler(
    IApplicationDbContext dbContext,
    IQRCodeService qrCodeService)
    : IRequestHandler<GenerateQRCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GenerateQRCodeCommand cmd, CancellationToken ct)
    {
        var bon = await dbContext.BonsEntree.FindAsync([cmd.BonEntreeId], ct);
        if (bon is null)
            return Result.Failure<string>(Error.NotFound("BonEntree", cmd.BonEntreeId));

        var qrData = JsonSerializer.Serialize(new
        {
            NumeroRef = bon.NumeroReference,
            Id = bon.Id,
            Type = "BEM"
        });

        var (_, base64, hash) = await qrCodeService.GenerateAsync(qrData, ct);

        bon.SetQRCode(qrData, base64, hash);
        await dbContext.SaveChangesAsync(ct);

        return base64;
    }
}
