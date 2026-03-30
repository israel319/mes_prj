using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Approbation dans le workflow d'un bon d'entrée.
/// EF maps Id → IdApprobation column.
/// </summary>
public sealed class Approbation : BaseEntity
{
    public int BonId { get; set; }
    public int OrdreEtape { get; set; }

    [MaxLength(100)]
    public string? NomEtape { get; set; }

    [MaxLength(50)]
    public string Decision { get; set; } = "En attente";

    public DateTime? DateAction { get; set; }

    [MaxLength(200)]
    public string? NomApprobateur { get; set; }

    [MaxLength(50)]
    public string? RoleApprobateur { get; set; }

    [MaxLength(1000)]
    public string? ReservesEventuelles { get; set; }

    private Approbation() { }

    public Approbation(int bonId, int ordreEtape, string? nomEtape)
    {
        BonId = bonId;
        OrdreEtape = ordreEtape;
        NomEtape = nomEtape;
    }

    public void Approuver(string nomApprobateur, string roleApprobateur, string? commentaire = null)
    {
        Decision = "Approuvé";
        DateAction = DateTime.Now;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = commentaire;
    }

    public void Rejeter(string nomApprobateur, string roleApprobateur, string motif)
    {
        Decision = "Rejeté";
        DateAction = DateTime.Now;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = motif;
    }

    public void Retourner(string nomApprobateur, string roleApprobateur, string motif)
    {
        Decision = "Retourné";
        DateAction = DateTime.Now;
        NomApprobateur = nomApprobateur;
        RoleApprobateur = roleApprobateur;
        ReservesEventuelles = motif;
    }
}
