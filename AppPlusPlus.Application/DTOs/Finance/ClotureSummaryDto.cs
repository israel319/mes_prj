namespace AppPlusPlus.Application.DTOs.Finance;

public class ClotureSummaryDto
{
    public int NbFactures { get; set; }
    public decimal TotalFactures { get; set; }
    public int NbPaiements { get; set; }
    public decimal TotalPaiements { get; set; }
    public bool DejaClotureExiste { get; set; }
}
