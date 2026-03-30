using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Admin.Departements;

// ── Create ────────────────────────────────────────────────────────────────

public sealed record CreateDepartementCommand(
    string Code,
    string Nom,
    string? Description,
    string? ResponsableLogin,
    string? ResponsableNom,
    string? ResponsableEmail) : IRequest<Result<int>>;

internal sealed class CreateDepartementHandler(IApplicationDbContext db)
    : IRequestHandler<CreateDepartementCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateDepartementCommand request, CancellationToken ct)
    {
        var exists = await db.Departements
            .AnyAsync(d => d.CodeDepartement == request.Code, ct);

        if (exists)
            return Result.Failure<int>(Error.Conflict("Departement.CodeExists",
                $"Un département avec le code '{request.Code}' existe déjà."));

        var entity = new Departement
        {
            CodeDepartement = request.Code,
            NomDepartement = request.Nom,
            Description = request.Description,
            ResponsableLogin = request.ResponsableLogin ?? string.Empty,
            ResponsableNom = request.ResponsableNom,
            ResponsableEmail = request.ResponsableEmail,
            EstActif = true,
            DateCreation = DateTime.Now
        };

        db.Departements.Add(entity);
        await db.SaveChangesAsync(ct);

        return Result.Success(entity.Id);
    }
}

// ── Update ────────────────────────────────────────────────────────────────

public sealed record UpdateDepartementCommand(
    int Id,
    string Code,
    string Nom,
    string? Description,
    string? ResponsableLogin,
    string? ResponsableNom,
    string? ResponsableEmail,
    bool EstActif) : IRequest<Result>;

internal sealed class UpdateDepartementHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateDepartementCommand, Result>
{
    public async Task<Result> Handle(UpdateDepartementCommand request, CancellationToken ct)
    {
        var entity = await db.Departements.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Failure(Error.NotFound("Departement", request.Id));

        var duplicate = await db.Departements
            .AnyAsync(d => d.CodeDepartement == request.Code && d.Id != request.Id, ct);

        if (duplicate)
            return Result.Failure(Error.Conflict("Departement.CodeExists",
                $"Un autre département utilise déjà le code '{request.Code}'."));

        entity.CodeDepartement = request.Code;
        entity.NomDepartement = request.Nom;
        entity.Description = request.Description;
        entity.ResponsableLogin = request.ResponsableLogin ?? string.Empty;
        entity.ResponsableNom = request.ResponsableNom;
        entity.ResponsableEmail = request.ResponsableEmail;
        entity.EstActif = request.EstActif;
        entity.DateModification = DateTime.Now;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
