using System.ComponentModel.DataAnnotations;

namespace LicenseKeyServer.Models;

public class KeyCheckRequest
{
    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string DeviceId { get; set; } = string.Empty;

    public string? ProductCode { get; set; }
}
