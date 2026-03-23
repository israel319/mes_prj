using KCCMaterialFlow.Infrastructure.Data;
using KCCMaterialFlow.Module.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace KCCMaterialFlow.Infrastructure.Performance;

/// <summary>
/// Requêtes compilées EF Core pour les entités fréquemment accédées.
/// INT-024: Implémenter compiled queries - EF Core compiled queries
/// Les compiled queries réduisent le temps de parsing LINQ et améliorent les performances.
/// </summary>
public static class CompiledQueries
{
    // === UTILISATEUR COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère un utilisateur par son login
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<Utilisateur?>> 
        GetUtilisateurByLogin = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string login) =>
                ctx.Set<Utilisateur>()
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login == login));

    /// <summary>
    /// Vérifie si un utilisateur est actif
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<bool>> 
        IsUtilisateurActif = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string login) =>
                ctx.Set<Utilisateur>()
                    .Any(u => u.Login == login && u.EstActif));

    // === DEPARTEMENT COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère un département par son code
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<Departement?>> 
        GetDepartementByCode = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string code) =>
                ctx.Set<Departement>()
                    .AsNoTracking()
                    .FirstOrDefault(d => d.CodeDepartement == code));

    // === BARRIERE COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère une barrière par son code
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<Barriere?>> 
        GetBarriereByCode = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string code) =>
                ctx.Set<Barriere>()
                    .AsNoTracking()
                    .FirstOrDefault(b => b.CodeBarriere == code));

    // === STATUT COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère un statut par son code
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<Statut?>> 
        GetStatutByCode = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string code) =>
                ctx.Set<Statut>()
                    .AsNoTracking()
                    .FirstOrDefault(s => s.CodeStatut == code));

    // === PARAMETRE COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère un paramètre système par sa clé
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<ParametreSysteme?>> 
        GetParametreByKey = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string cle) =>
                ctx.Set<ParametreSysteme>()
                    .AsNoTracking()
                    .FirstOrDefault(p => p.Cle == cle));

    // === TYPE MATERIEL COMPILED QUERIES ===
    
    /// <summary>
    /// Récupère un type de matériel par son code
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, Task<TypeMateriel?>> 
        GetTypeMaterielByCode = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string code) =>
                ctx.Set<TypeMateriel>()
                    .AsNoTracking()
                    .FirstOrDefault(t => t.CodeType == code));

    // === AUDIT LOG COMPILED QUERIES ===
    
    /// <summary>
    /// Compte les actions d'audit pour un utilisateur
    /// </summary>
    public static readonly Func<KCCMaterialFlowDbContext, string, DateTime, DateTime, Task<int>> 
        GetAuditCountForUser = EF.CompileAsyncQuery(
            (KCCMaterialFlowDbContext ctx, string login, DateTime dateDebut, DateTime dateFin) =>
                ctx.Set<AuditLog>()
                    .Count(a => a.UtilisateurLogin == login && 
                               a.DateAction >= dateDebut && 
                               a.DateAction <= dateFin));
}
