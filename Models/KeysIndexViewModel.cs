namespace LicenseKeyServer.Models;

public class KeysIndexViewModel
{
    public KeyFilterViewModel Filter { get; set; } = new();
    public List<LicenseKey> Items { get; set; } = new();
}
