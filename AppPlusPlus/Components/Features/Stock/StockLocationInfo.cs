namespace AppPlusPlus.Components.Features.Stock;

public class StockLocationInfo
{
    public int StockId { get; set; }
    public int IdLocalisation { get; set; }
    public string LocalisationName { get; set; } = "";
    public int Qte { get; set; }
    public int Seuil { get; set; }
}
