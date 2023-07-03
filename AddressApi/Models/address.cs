namespace AddressApi.Models;

public class Address
{
    public long Id { get; set; }
    public string? Straat { get; set; }
    public string? Huisnummer { get; set; }
    public string? Postcode { get; set; }
    public string? Plaats { get; set; }
    public string? Land { get; set; }
}