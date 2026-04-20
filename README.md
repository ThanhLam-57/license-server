# LicenseKeyServer

ASP.NET Core MVC + API để quản lý và kiểm tra license key cho game Unity.

## Có gì sẵn trong project

- UI web quản lý key
- Tạo key hàng loạt
- Sửa / xóa key
- Khóa / mở khóa key
- Reset thiết bị đã bind
- Lọc theo key, device, product, trạng thái
- API cho Unity gọi: `POST /api/license/check`
- Dùng MySQL qua EF Core + Pomelo

## Cấu trúc chính

- `Controllers/KeysController.cs`: UI quản lý key
- `Controllers/ApiLicenseController.cs`: API check key cho Unity
- `Models/LicenseKey.cs`: model key
- `Services/KeyGenerator.cs`: tạo key ngẫu nhiên
- `Views/Keys/*`: giao diện Razor
- `Data/AppDbContext.cs`: EF Core DbContext

## 1) Cài .NET SDK

Cài .NET 8 SDK.

## 2) Tạo MySQL database

```sql
CREATE DATABASE license_key_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

## 3) Sửa connection string

Mở `appsettings.json` và sửa:

```json
"DefaultConnection": "server=localhost;port=3306;database=license_key_db;user=YOUR_DB_USER;password=YOUR_DB_PASSWORD"
```

## 4) Restore và chạy

```bash
dotnet restore
dotnet run
```

App sẽ tự `EnsureCreated()` bảng khi chạy lần đầu.

## 5) URL mặc định

- UI: `https://localhost:5001/` hoặc `http://localhost:5000/`
- API: `POST /api/license/check`

## Payload từ Unity

```json
{
  "key": "ALD-XXXX-XXXX-XXXX-XXXX",
  "deviceId": "MAY-ABC-123",
  "productCode": "TIKTOK-PK-GAME"
}
```

## Response mẫu

```json
{
  "success": true,
  "code": "VALID",
  "message": "Key hợp lệ.",
  "expireAt": null,
  "productCode": "TIKTOK-PK-GAME"
}
```

## Logic check key

- Không có key => `NOT_FOUND`
- Sai product => `PRODUCT_MISMATCH`
- Bị khóa => `BLOCKED`
- Hết hạn => `EXPIRED`
- Chưa bind máy => tự bind máy đầu tiên và trả `ACTIVATED`
- Đúng máy đã bind => `VALID`
- Khác máy => `DEVICE_MISMATCH`

## Gợi ý dùng với Unity

Unity gửi POST JSON tới `/api/license/check`.

Ví dụ body:

```csharp
[System.Serializable]
public class LicenseCheckRequest
{
    public string key;
    public string deviceId;
    public string productCode;
}
```

## Nên nâng cấp sau

- Đăng nhập admin
- JWT/HMAC cho API
- Rate limit
- Audit log
- Hash key thay vì lưu plain text
- Phân quyền nhiều game
