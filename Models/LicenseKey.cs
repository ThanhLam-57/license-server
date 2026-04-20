using System.ComponentModel.DataAnnotations;

namespace LicenseKeyServer.Models;

public class LicenseKey
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "License Key")]
    public string KeyValue { get; set; } = string.Empty;

    [Display(Name = "Device ID")]
    public string? DeviceId { get; set; }

    [Display(Name = "Sản phẩm / Game")]
    public string? ProductCode { get; set; }

    [Display(Name = "Ngày hết hạn")]
    [DataType(DataType.DateTime)]
    public DateTime? ExpireAt { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Ngày kích hoạt")]
    public DateTime? ActivatedAt { get; set; }

    [Display(Name = "Số lần kích hoạt")]
    public int ActivationCount { get; set; }

    [Display(Name = "Trạng thái")]
    public LicenseStatus Status { get; set; } = LicenseStatus.Active;

    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }

    public bool IsExpired => ExpireAt.HasValue && ExpireAt.Value < DateTime.UtcNow;
    public bool IsBound => !string.IsNullOrWhiteSpace(DeviceId);
}

public enum LicenseStatus
{
    Active = 1,
    Blocked = 2,
    Disabled = 3
}
