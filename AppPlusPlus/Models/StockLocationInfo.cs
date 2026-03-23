namespace AppPlusPlus.Models;

/// <summary>
/// View model for displaying stock information per location
/// </summary>
public class StockLocationInfo
{
    public int StockId { get; set; }
    public int IdLocalisation { get; set; }
    public string LocalisationName { get; set; } = "";
    public int Qte { get; set; }
    public int Seuil { get; set; }
}
