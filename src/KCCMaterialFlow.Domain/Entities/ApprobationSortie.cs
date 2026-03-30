using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Approbation dans le workflow d'un bon de sortie.
/// EF maps Id → IdApprobation column.
/// </summary>
public sealed class ApprobationSortie : BaseEntity
{
    public int BonId { get; set; }
    public int OrdreEtape { get; set; }

    [MaxLength(100)]
    public string? NomEtape { get; set; }

    [MaxLength(50)]
    public string Decision { get; set; } = "En attente";

    public DateTime? DateAction { get; set; }

    [MaxLength(100)]
    public string? ApprobateurLogin { get; set; }

    [MaxLength(200)]
    public string? NomApprobateur { get; set; }

    [MaxLength(50)]
    public string? RoleApprobateur { get; set; }

    [MaxLength(1000)]
    public string? ReservesEventuelles { get; set; }

    private ApprobationSortie() { }

    public ApprobationSortie(int bonId, int ordreEtape, string? nomEtape)
    {
        BonId = bonId;
        OrdreEtape = ordreEtape;
        NomEtape = nomEtape;
    }

    public void Approuver(string approbateurLogin, string nomApprobateur, string roleApprobateur, string? commentaire = null)
    {
        Decision = "Approuvé";
        DateAction = DateTime.Now;
        ApprobateurLogin = approbateurLogin;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = commentaire;
    }

    public void Rejeter(string approbateurLogin, string nomApprobateur, string roleApprobateur, string motif)
    {
        Decision = "Rejeté";
        DateAction = DateTime.Now;
        ApprobateurLogin = approbateurLogin;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = motif;
    }

    public void Retourner(string approbateurLogin, string nomApprobateur, string roleApprobateur, string motif)
    {
        Decision = "Retourné";
        DateAction = DateTime.Now;
        ApprobateurLogin = approbateurLogin;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = motif;
    }
}
