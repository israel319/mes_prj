using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Performance;

/// <summary>
/// Requêtes optimisées avec projections DTO.
/// INT-023: Optimiser queries listes - Projection DTO dans queries
/// Ces méthodes retournent uniquement les données nécessaires via Select(),
/// évitant le chargement d'entités complètes.
/// </summary>
public static class OptimizedQueries
{
    // === UTILISATEUR OPTIMIZED QUERIES ===
    
    /// <summary>
    /// Récupère la liste des utilisateurs avec projection DTO
    /// </summary>
    public static IQueryable<UtilisateurListDto> GetUtilisateursListQuery(
        KCCMaterialFlowDbContext ctx,
        string? searchTerm = null,
        bool? estActif = null)
    {
        var query = ctx.Set<Utilisateur>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToUpper();
            query = query.Where(u => 
                u.Login.ToUpper().Contains(term) || 
                u.NomComplet.ToUpper().Contains(term));
        }

        if (estActif.HasValue)
        {
            query = query.Where(u => u.EstActif == estActif.Value);
        }

        return query
            .OrderBy(u => u.NomComplet)
            .Select(u => new UtilisateurListDto
            {
                IdUtilisateur = u.IdUtilisateur,
                Login = u.Login,
                NomComplet = u.NomComplet,
                Matricule = null, // Pas de matricule dans l'entité actuelle
                Email = u.Email,
                NomDepartement = u.Departement, // C'est une string dans l'entité
                EstActif = u.EstActif,
                DateDerniereConnexion = u.DerniereConnexion
            });
    }

    /// <summary>
    /// Récupère les utilisateurs pour dropdown avec projection DTO
    /// </summary>
    public static IQueryable<UtilisateurLookupDto> GetUtilisateursLookupQuery(
        KCCMaterialFlowDbContext ctx,
        bool actifsSeulement = true)
    {
        var query = ctx.Set<Utilisateur>().AsNoTracking();

        if (actifsSeulement)
        {
            query = query.Where(u => u.EstActif);
        }

        return query
            .OrderBy(u => u.NomComplet)
            .Select(u => new UtilisateurLookupDto
            {
                IdUtilisateur = u.IdUtilisateur,
                Login = u.Login,
                NomComplet = u.NomComplet
            });
    }

    // === DEPARTEMENT OPTIMIZED QUERIES ===
    
    /// <summary>
    /// Récupère la liste des départements avec projection DTO
    /// </summary>
    public static IQueryable<DepartementListDto> GetDepartementsListQuery(
        KCCMaterialFlowDbContext ctx,
        bool? estActif = null)
    {
        var query = ctx.Set<Departement>().AsNoTracking();

        if (estActif.HasValue)
        {
            query = query.Where(d => d.EstActif == estActif.Value);
        }

        return query
            .OrderBy(d => d.NomDepartement)
            .Select(d => new DepartementListDto
            {
                IdDepartement = d.IdDepartement,
                CodeDepartement = d.CodeDepartement,
                NomDepartement = d.NomDepartement,
                NomResponsable = d.ResponsableNom,
                NombreUtilisateurs = 0, // Calculé séparément si nécessaire
                EstActif = d.EstActif
            });
    }

    /// <summary>
    /// Récupère les départements pour dropdown avec projection DTO
    /// </summary>
    public static IQueryable<DepartementLookupDto> GetDepartementsLookupQuery(
        KCCMaterialFlowDbContext ctx,
        bool actifsSeulement = true)
    {
        var query = ctx.Set<Departement>().AsNoTracking();

        if (actifsSeulement)
        {
            query = query.Where(d => d.EstActif);
        }

        return query
            .OrderBy(d => d.NomDepartement)
            .Select(d => new DepartementLookupDto
            {
                IdDepartement = d.IdDepartement,
                CodeDepartement = d.CodeDepartement,
                NomDepartement = d.NomDepartement
            });
    }

    // === AUDIT LOG OPTIMIZED QUERIES ===
    
    /// <summary>
    /// Récupère la liste des audit logs avec projection DTO
    /// </summary>
    public static IQueryable<AuditLogListDto> GetAuditLogsListQuery(
        KCCMaterialFlowDbContext ctx,
        DateTime? dateDebut = null,
        DateTime? dateFin = null,
        string? utilisateurLogin = null,
        string? typeAction = null,
        string? categorie = null)
    {
        var query = ctx.Set<AuditLog>().AsNoTracking();

        if (dateDebut.HasValue)
        {
            query = query.Where(a => a.DateAction >= dateDebut.Value);
        }

        if (dateFin.HasValue)
        {
            query = query.Where(a => a.DateAction <= dateFin.Value.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(utilisateurLogin))
        {
            var term = utilisateurLogin.ToUpper();
            query = query.Where(a => a.UtilisateurLogin.ToUpper().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(typeAction))
        {
            query = query.Where(a => a.TypeAction == typeAction);
        }

        if (!string.IsNullOrWhiteSpace(categorie))
        {
            query = query.Where(a => a.Categorie == categorie);
        }

        return query
            .OrderByDescending(a => a.DateAction)
            .Select(a => new AuditLogListDto
            {
                IdAuditLog = a.IdAuditLog,
                DateAction = a.DateAction,
                UtilisateurLogin = a.UtilisateurLogin,
                UtilisateurNom = a.UtilisateurNom,
                TypeAction = a.TypeAction,
                Categorie = a.Categorie,
                Description = a.Description,
                Niveau = a.Niveau,
                HasDetails = !string.IsNullOrEmpty(a.Details) || 
                            !string.IsNullOrEmpty(a.AncienneValeur) || 
                            !string.IsNullOrEmpty(a.NouvelleValeur)
            });
    }

    // === STATISTIQUES OPTIMIZED QUERIES ===
    
    /// <summary>
    /// Récupère le nombre total d'utilisateurs actifs (optimisé)
    /// </summary>
    public static async Task<int> GetActiveUsersCountAsync(
        KCCMaterialFlowDbContext ctx,
        CancellationToken cancellationToken = default)
    {
        return await ctx.Set<Utilisateur>()
            .CountAsync(u => u.EstActif, cancellationToken);
    }
}
