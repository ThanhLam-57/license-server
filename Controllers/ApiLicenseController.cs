using LicenseKeyServer.Data;
using LicenseKeyServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicenseKeyServer.Controllers;

[ApiController]
[Route("api/license")]
public class ApiLicenseController : ControllerBase
{
    private readonly AppDbContext _db;

    public ApiLicenseController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("check")]
    public async Task<ActionResult<KeyCheckResponse>> Check([FromBody] KeyCheckRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new KeyCheckResponse
            {
                Success = false,
                Code = "INVALID_REQUEST",
                Message = "Thiếu key hoặc device id."
            });
        }

        var normalizedKey = request.Key.Trim().ToUpperInvariant();
        var normalizedDevice = request.DeviceId.Trim();
        var license = await _db.LicenseKeys.FirstOrDefaultAsync(x => x.KeyValue == normalizedKey);

        if (license == null)
        {
            return Ok(new KeyCheckResponse
            {
                Success = false,
                Code = "NOT_FOUND",
                Message = "Key không tồn tại."
            });
        }

        if (!string.IsNullOrWhiteSpace(request.ProductCode)
            && !string.IsNullOrWhiteSpace(license.ProductCode)
            && !string.Equals(request.ProductCode.Trim(), license.ProductCode, StringComparison.OrdinalIgnoreCase))
        {
            return Ok(new KeyCheckResponse
            {
                Success = false,
                Code = "PRODUCT_MISMATCH",
                Message = "Key không thuộc game này."
            });
        }

        if (license.Status == LicenseStatus.Blocked || license.Status == LicenseStatus.Disabled)
        {
            return Ok(new KeyCheckResponse
            {
                Success = false,
                Code = "BLOCKED",
                Message = "Key đã bị khóa hoặc vô hiệu hóa."
            });
        }

        if (license.IsExpired)
        {
            return Ok(new KeyCheckResponse
            {
                Success = false,
                Code = "EXPIRED",
                Message = "Key đã hết hạn.",
                ExpireAt = license.ExpireAt,
                ProductCode = license.ProductCode
            });
        }

        if (string.IsNullOrWhiteSpace(license.DeviceId))
        {
            license.DeviceId = normalizedDevice;
            license.ActivatedAt = DateTime.UtcNow;
            license.ActivationCount = 1;
            await _db.SaveChangesAsync();

            return Ok(new KeyCheckResponse
            {
                Success = true,
                Code = "ACTIVATED",
                Message = "Kích hoạt thành công.",
                ExpireAt = license.ExpireAt,
                ProductCode = license.ProductCode
            });
        }

        if (!string.Equals(license.DeviceId, normalizedDevice, StringComparison.Ordinal))
        {
            return Ok(new KeyCheckResponse
            {
                Success = false,
                Code = "DEVICE_MISMATCH",
                Message = "Key đã được kích hoạt trên máy khác.",
                ExpireAt = license.ExpireAt,
                ProductCode = license.ProductCode
            });
        }

        license.ActivationCount += 1;
        await _db.SaveChangesAsync();

        return Ok(new KeyCheckResponse
        {
            Success = true,
            Code = "VALID",
            Message = "Key hợp lệ.",
            ExpireAt = license.ExpireAt,
            ProductCode = license.ProductCode
        });
    }
}
