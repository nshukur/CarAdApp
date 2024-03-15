namespace CarAdApp.Models;

public class CarUpdateDto
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? ProductionYear { get; set; }
    public double? EngineVolume { get; set; }
    public int? Mileage { get; set; }
    public decimal? Price { get; set; }
    public string? Color { get; set; }
    public BanType? BanType { get; set; }
    public FuelType? FuelType { get; set; }
    public Gearbox? Gearbox { get; set; }
    public string? PictureUrl { get; set; }
}