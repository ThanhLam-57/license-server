using System.ComponentModel.DataAnnotations;

namespace LicenseKeyServer.Models;

public class CreateKeysViewModel
{
    [Range(1, 1000)]
    [Display(Name = "Số lượng key")]
    public int Quantity { get; set; } = 10;

    [Range(4, 8)]
    [Display(Name = "Số block")]
    public int GroupCount { get; set; } = 4;

    [Range(3, 8)]
    [Display(Name = "Ký tự mỗi block")]
    public int CharactersPerGroup { get; set; } = 4;

    [Display(Name = "Tiền tố")]
    public string? Prefix { get; set; } = "ALD";

    [Display(Name = "Sản phẩm / Game")]
    public string? ProductCode { get; set; }

    [Display(Name = "Ngày hết hạn")]
    [DataType(DataType.DateTime)]
    public DateTime? ExpireAt { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }
}
