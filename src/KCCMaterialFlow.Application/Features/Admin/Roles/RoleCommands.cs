using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Entities;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Admin.Roles;

// ── Create ────────────────────────────────────────────────────────────────

public sealed record CreateRoleCommand(
    string Code,
    string Nom,
    string? Description,
    int NiveauPriorite = 0) : IRequest<Result<int>>;

internal sealed class CreateRoleHandler(IApplicationDbContext db)
    : IRequestHandler<CreateRoleCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateRoleCommand request, CancellationToken ct)
    {
        var exists = await db.Roles
            .AnyAsync(r => r.CodeRole == request.Code, ct);

        if (exists)
            return Result.Failure<int>(Error.Conflict("Role.CodeExists",
                $"Un rôle avec le code '{request.Code}' existe déjà."));

        var entity = new Role
        {
            CodeRole = request.Code,
            NomRole = request.Nom,
            Description = request.Description,
            NiveauPriorite = request.NiveauPriorite,
            EstActif = true,
            DateCreation = DateTime.Now
        };

        db.Roles.Add(entity);
        await db.SaveChangesAsync(ct);

        return Result.Success(entity.Id);
    }
}

// ── Update ────────────────────────────────────────────────────────────────

public sealed record UpdateRoleCommand(
    int Id,
    string Code,
    string Nom,
    string? Description,
    bool EstActif,
    int NiveauPriorite = 0) : IRequest<Result>;

internal sealed class UpdateRoleHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var entity = await db.Roles.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Failure(Error.NotFound("Role", request.Id));

        var duplicate = await db.Roles
            .AnyAsync(r => r.CodeRole == request.Code && r.Id != request.Id, ct);

        if (duplicate)
            return Result.Failure(Error.Conflict("Role.CodeExists",
                $"Un autre rôle utilise déjà le code '{request.Code}'."));

        entity.CodeRole = request.Code;
        entity.NomRole = request.Nom;
        entity.Description = request.Description;
        entity.NiveauPriorite = request.NiveauPriorite;
        entity.EstActif = request.EstActif;
        entity.DateModification = DateTime.Now;

        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── Delete ────────────────────────────────────────────────────────────────

public sealed record DeleteRoleCommand(int Id) : IRequest<Result>;

internal sealed class DeleteRoleHandler(IApplicationDbContext db)
    : IRequestHandler<DeleteRoleCommand, Result>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken ct)
    {
        var entity = await db.Roles.FindAsync([request.Id], ct);
        if (entity is null)
            return Result.Failure(Error.NotFound("Role", request.Id));

        if (entity.EstSysteme)
            return Result.Failure(Error.Validation("Role.SystemRole",
                "Impossible de supprimer un rôle système."));

        var hasUsers = await db.UtilisateurRoles
            .AnyAsync(ur => ur.IdRole == request.Id, ct);

        if (hasUsers)
            return Result.Failure(Error.Validation("Role.HasUsers",
                "Impossible de supprimer un rôle assigné à des utilisateurs."));

        db.Roles.Remove(entity);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── AssignRoleToUser ──────────────────────────────────────────────────────

public sealed record AssignRoleToUserCommand(int UserId, int RoleId) : IRequest<Result>;

internal sealed class AssignRoleToUserHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<AssignRoleToUserCommand, Result>
{
    public async Task<Result> Handle(AssignRoleToUserCommand request, CancellationToken ct)
    {
        var userExists = await db.Utilisateurs.AnyAsync(u => u.Id == request.UserId, ct);
        if (!userExists)
            return Result.Failure(Error.NotFound("Utilisateur", request.UserId));

        var roleExists = await db.Roles.AnyAsync(r => r.Id == request.RoleId, ct);
        if (!roleExists)
            return Result.Failure(Error.NotFound("Role", request.RoleId));

        var alreadyAssigned = await db.UtilisateurRoles
            .AnyAsync(ur => ur.IdUtilisateur == request.UserId && ur.IdRole == request.RoleId, ct);

        if (alreadyAssigned)
            return Result.Failure(Error.Conflict("Role.AlreadyAssigned",
                "Ce rôle est déjà assigné à cet utilisateur."));

        var entry = new UtilisateurRole
        {
            IdUtilisateur = request.UserId,
            IdRole = request.RoleId,
            DateAttribution = DateTime.Now,
            AttribueParLogin = currentUser.GetUserLogin()
        };

        db.UtilisateurRoles.Add(entry);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── RemoveRoleFromUser ────────────────────────────────────────────────────

public sealed record RemoveRoleFromUserCommand(int UserId, int RoleId) : IRequest<Result>;

internal sealed class RemoveRoleFromUserHandler(IApplicationDbContext db)
    : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken ct)
    {
        var entry = await db.UtilisateurRoles
            .FirstOrDefaultAsync(ur => ur.IdUtilisateur == request.UserId
                                    && ur.IdRole == request.RoleId, ct);

        if (entry is null)
            return Result.Failure(Error.NotFound("UtilisateurRole",
                $"UserId={request.UserId}, RoleId={request.RoleId}"));

        db.UtilisateurRoles.Remove(entry);
        await db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
