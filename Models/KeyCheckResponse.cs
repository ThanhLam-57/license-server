namespace LicenseKeyServer.Models;

public class KeyCheckResponse
{
    public bool Success { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime? ExpireAt { get; set; }
    public string? ProductCode { get; set; }
}
