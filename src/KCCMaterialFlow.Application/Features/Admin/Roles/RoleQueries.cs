using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Admin.Roles;

// ── DTO ───────────────────────────────────────────────────────────────────

public sealed record RoleDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int NiveauPriorite { get; init; }
    public bool EstActif { get; init; }
    public bool EstSysteme { get; init; }
    public DateTime DateCreation { get; init; }
    public DateTime? DateModification { get; init; }
}

// ── GetAll ─────────────────────────────────────────────────────────────────

public sealed record GetAllRolesQuery() : IRequest<Result<IReadOnlyList<RoleDto>>>;

internal sealed class GetAllRolesHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(
        GetAllRolesQuery request, CancellationToken ct)
    {
        var list = await db.Roles
            .AsNoTracking()
            .OrderBy(r => r.NomRole)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Code = r.CodeRole,
                Nom = r.NomRole,
                Description = r.Description,
                NiveauPriorite = r.NiveauPriorite,
                EstActif = r.EstActif,
                EstSysteme = r.EstSysteme,
                DateCreation = r.DateCreation,
                DateModification = r.DateModification
            })
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<RoleDto>>(list);
    }
}

// ── GetRolesForUser ────────────────────────────────────────────────────────

public sealed record GetRolesForUserQuery(int UserId)
    : IRequest<Result<IReadOnlyList<RoleDto>>>;

internal sealed class GetRolesForUserHandler(IApplicationDbContext db)
    : IRequestHandler<GetRolesForUserQuery, Result<IReadOnlyList<RoleDto>>>
{
    public async Task<Result<IReadOnlyList<RoleDto>>> Handle(
        GetRolesForUserQuery request, CancellationToken ct)
    {
        var list = await db.UtilisateurRoles
            .AsNoTracking()
            .Where(ur => ur.IdUtilisateur == request.UserId)
            .Join(db.Roles, ur => ur.IdRole, r => r.Id, (_, r) => r)
            .OrderBy(r => r.NomRole)
            .Select(r => new RoleDto
            {
                Id = r.Id,
                Code = r.CodeRole,
                Nom = r.NomRole,
                Description = r.Description,
                NiveauPriorite = r.NiveauPriorite,
                EstActif = r.EstActif,
                EstSysteme = r.EstSysteme,
                DateCreation = r.DateCreation,
                DateModification = r.DateModification
            })
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<RoleDto>>(list);
    }
}
