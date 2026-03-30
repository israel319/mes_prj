using System.ComponentModel.DataAnnotations;
using KCCMaterialFlow.Domain.Common;

namespace KCCMaterialFlow.Domain.Entities;

/// <summary>
/// Historique consolide des scans pour vue "Identification".
/// EF maps Id -> IdHistorique column.
/// </summary>
public sealed class HistoriqueScan : BaseEntity
{
    public int ScanId { get; private set; }
    public DateTime DateHeureMouvement { get; private set; } = DateTime.Now;

    [MaxLength(20)]
    public string TypeMouvement { get; private set; } = "Sortie";

    [MaxLength(10)]
    public string TypeBon { get; private set; } = "BSM";

    [MaxLength(30)]
    public string NumeroReferenceBon { get; private set; } = string.Empty;

    [MaxLength(20)]
    public string CodeBarriere { get; private set; } = string.Empty;

    [MaxLength(100)]
    public string? NomBarriere { get; private set; }

    [MaxLength(30)]
    public string? Direction { get; private set; }

    [MaxLength(100)]
    public string? Departement { get; private set; }

    [MaxLength(200)]
    public string? Provenance { get; private set; }

    [MaxLength(200)]
    public string? Destination { get; private set; }

    public int NombreMateriels { get; private set; }

    [MaxLength(2000)]
    public string? ResumeMateriels { get; private set; }

    public string? MaterielsJson { get; private set; }

    [MaxLength(30)]
    public string StatutScan { get; private set; } = "Valid";

    public bool PassageAutorise { get; private set; } = true;

    [MaxLength(100)]
    public string AgentLogin { get; private set; } = string.Empty;

    [MaxLength(200)]
    public string? AgentNom { get; private set; }

    [MaxLength(200)]
    public string? NomDemandeur { get; private set; }

    [MaxLength(50)]
    public string? MatriculeVehicule { get; private set; }

    [MaxLength(1000)]
    public string? Observations { get; private set; }

    public bool AnomalieSignalee { get; private set; }
    public int NombreAnomalies { get; private set; }

    private HistoriqueScan() { }

    public HistoriqueScan(
        int scanId, string typeMouvement, string typeBon,
        string numeroReferenceBon, string codeBarriere,
        string agentLogin, string statutScan,
        bool passageAutorise = true, string? nomBarriere = null,
        string? direction = null, string? agentNom = null,
        string? nomDemandeur = null, string? provenance = null,
        string? destination = null, int nombreMateriels = 0,
        string? resumeMateriels = null, string? materielsJson = null,
        string? matriculeVehicule = null, string? observations = null,
        string? departement = null)
    {
        ScanId = scanId;
        TypeMouvement = typeMouvement;
        TypeBon = typeBon;
        NumeroReferenceBon = numeroReferenceBon;
        CodeBarriere = codeBarriere;
        AgentLogin = agentLogin;
        StatutScan = statutScan;
        PassageAutorise = passageAutorise;
        NomBarriere = nomBarriere;
        Direction = direction;
        AgentNom = agentNom;
        NomDemandeur = nomDemandeur;
        Provenance = provenance;
        Destination = destination;
        NombreMateriels = nombreMateriels;
        ResumeMateriels = resumeMateriels;
        MaterielsJson = materielsJson;
        MatriculeVehicule = matriculeVehicule;
        Observations = observations;
        Departement = departement;
    }

    public void SignalerAnomalie(int count = 1)
    {
        AnomalieSignalee = true;
        NombreAnomalies += count;
    }
}
