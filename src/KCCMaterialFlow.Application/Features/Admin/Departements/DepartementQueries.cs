using KCCMaterialFlow.Application.Common.Interfaces;
using KCCMaterialFlow.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Application.Features.Admin.Departements;

// ── DTO ───────────────────────────────────────────────────────────────────

public sealed record DepartementDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Nom { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string ResponsableLogin { get; init; } = string.Empty;
    public string? ResponsableNom { get; init; }
    public string? ResponsableEmail { get; init; }
    public bool EstActif { get; init; }
    public DateTime DateCreation { get; init; }
    public DateTime? DateModification { get; init; }
}

// ── GetAll ─────────────────────────────────────────────────────────────────

public sealed record GetAllDepartementsQuery(bool ActiveOnly = false)
    : IRequest<Result<IReadOnlyList<DepartementDto>>>;

internal sealed class GetAllDepartementsHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllDepartementsQuery, Result<IReadOnlyList<DepartementDto>>>
{
    public async Task<Result<IReadOnlyList<DepartementDto>>> Handle(
        GetAllDepartementsQuery request, CancellationToken ct)
    {
        var query = db.Departements.AsNoTracking();

        if (request.ActiveOnly)
            query = query.Where(d => d.EstActif);

        var list = await query
            .OrderBy(d => d.NomDepartement)
            .Select(d => new DepartementDto
            {
                Id = d.Id,
                Code = d.CodeDepartement,
                Nom = d.NomDepartement,
                Description = d.Description,
                ResponsableLogin = d.ResponsableLogin,
                ResponsableNom = d.ResponsableNom,
                ResponsableEmail = d.ResponsableEmail,
                EstActif = d.EstActif,
                DateCreation = d.DateCreation,
                DateModification = d.DateModification
            })
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<DepartementDto>>(list);
    }
}

// ── GetById ────────────────────────────────────────────────────────────────

public sealed record GetDepartementByIdQuery(int Id)
    : IRequest<Result<DepartementDto>>;

internal sealed class GetDepartementByIdHandler(IApplicationDbContext db)
    : IRequestHandler<GetDepartementByIdQuery, Result<DepartementDto>>
{
    public async Task<Result<DepartementDto>> Handle(
        GetDepartementByIdQuery request, CancellationToken ct)
    {
        var dto = await db.Departements
            .AsNoTracking()
            .Where(d => d.Id == request.Id)
            .Select(d => new DepartementDto
            {
                Id = d.Id,
                Code = d.CodeDepartement,
                Nom = d.NomDepartement,
                Description = d.Description,
                ResponsableLogin = d.ResponsableLogin,
                ResponsableNom = d.ResponsableNom,
                ResponsableEmail = d.ResponsableEmail,
                EstActif = d.EstActif,
                DateCreation = d.DateCreation,
                DateModification = d.DateModification
            })
            .FirstOrDefaultAsync(ct);

        return dto is null
            ? Result.Failure<DepartementDto>(Error.NotFound("Departement", request.Id))
            : Result.Success(dto);
    }
}
