namespace KCCMaterialFlow.Domain.Enums;

/// <summary>
/// Détermine le chemin d'approbation d'un bon de sortie en fonction du type de matériel.
/// </summary>
public enum WorkflowRoutage
{
    /// <summary>Chaîne standard : Supt → GM → OPJ → Identification.</summary>
    Standard = 0,

    /// <summary>Matériel IT : Supt → GM → Département IT → OPJ → Identification.</summary>
    IT = 1,

    /// <summary>Matériel environnemental (résidu, radioprotection, modification) :
    /// Supt → GM → Département Environnement → OPJ → Identification.</summary>
    Environment = 2
}
