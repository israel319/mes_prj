namespace AppPlusPlus.Application.DTOs.Vente;

/// <summary>
/// Flat row used to display factures in grids (VenteHub tab 0).
/// </summary>
public class FactRowDto
{
    public int Id { get; set; }
    public string Client { get; set; } = "";
    public string Articles { get; set; } = "";
    public double TotalQte { get; set; }
    public double Total { get; set; }
    public double Paye { get; set; }
    public double Solde { get; set; }
    public DateOnly Date { get; set; }
    public int Status { get; set; }
    public string Login { get; set; } = "";

    /// <summary>
    /// Computed display status: 0=Brouillon, 1=Validee, 2=Payee, 3=Annulee, 4=Partiel.
    /// </summary>
    public int DisplayStatus => Status == 3 ? 3 : Status == 0 ? 0 : Solde <= 0 ? 2 : Paye > 0 ? 4 : 1;
}
