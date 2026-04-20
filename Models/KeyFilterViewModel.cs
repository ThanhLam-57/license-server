namespace LicenseKeyServer.Models;

public class KeyFilterViewModel
{
    public string? Search { get; set; }
    public LicenseStatus? Status { get; set; }
    public bool? BoundOnly { get; set; }
}
