using LicenseKeyServer.Data;
using LicenseKeyServer.Models;
using LicenseKeyServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace LicenseKeyServer.Controllers;

[Authorize]
public class KeysController : Controller
{
    private readonly AppDbContext _db;
    private readonly IKeyGenerator _keyGenerator;

    public KeysController(AppDbContext db, IKeyGenerator keyGenerator)
    {
        _db = db;
        _keyGenerator = keyGenerator;
    }

    public async Task<IActionResult> Index(KeyFilterViewModel filter)
    {
        var query = _db.LicenseKeys.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(x =>
                x.KeyValue.Contains(search) ||
                (x.DeviceId != null && x.DeviceId.Contains(search)) ||
                (x.ProductCode != null && x.ProductCode.Contains(search)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.BoundOnly == true)
        {
            query = query.Where(x => x.DeviceId != null && x.DeviceId != "");
        }

        ViewBag.Total = await query.CountAsync();
        ViewBag.Active = await _db.LicenseKeys.CountAsync(x => x.Status == LicenseStatus.Active);
        ViewBag.Blocked = await _db.LicenseKeys.CountAsync(x => x.Status == LicenseStatus.Blocked);
        ViewBag.Bound = await _db.LicenseKeys.CountAsync(x => x.DeviceId != null && x.DeviceId != "");

        var items = await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        return View(new KeysIndexViewModel
        {
            Filter = filter,
            Items = items
        });
    }

    public IActionResult Create()
    {
        return View(new CreateKeysViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKeysViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var created = new List<string>();

        for (var i = 0; i < model.Quantity; i++)
        {
            string keyValue;
            do
            {
                keyValue = _keyGenerator.Generate(model.Prefix, model.GroupCount, model.CharactersPerGroup);
            }
            while (await _db.LicenseKeys.AnyAsync(x => x.KeyValue == keyValue));

            _db.LicenseKeys.Add(new LicenseKey
            {
                KeyValue = keyValue,
                ProductCode = model.ProductCode?.Trim(),
                ExpireAt = model.ExpireAt?.ToUniversalTime(),
                Note = model.Note,
                Status = LicenseStatus.Active,
                CreatedAt = DateTime.UtcNow
            });

            created.Add(keyValue);
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Đã tạo {created.Count} key.";
        TempData["GeneratedKeys"] = string.Join(Environment.NewLine, created);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.LicenseKeys.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LicenseKey model)
    {
        var item = await _db.LicenseKeys.FindAsync(id);
        if (item == null) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        item.ProductCode = model.ProductCode?.Trim();
        item.ExpireAt = model.ExpireAt?.ToUniversalTime();
        item.Status = model.Status;
        item.Note = model.Note;
        item.KeyValue = model.KeyValue.Trim().ToUpperInvariant();

        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật key.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetDevice(int id)
    {
        var item = await _db.LicenseKeys.FindAsync(id);
        if (item == null) return NotFound();

        item.DeviceId = null;
        item.ActivatedAt = null;
        item.ActivationCount = 0;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã reset thiết bị cho key.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBlock(int id)
    {
        var item = await _db.LicenseKeys.FindAsync(id);
        if (item == null) return NotFound();

        item.Status = item.Status == LicenseStatus.Blocked ? LicenseStatus.Active : LicenseStatus.Blocked;
        await _db.SaveChangesAsync();

        TempData["Success"] = item.Status == LicenseStatus.Blocked ? "Đã khóa key." : "Đã mở khóa key.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.LicenseKeys.FindAsync(id);
        if (item == null) return NotFound();

        _db.LicenseKeys.Remove(item);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đã xóa key.";
        return RedirectToAction(nameof(Index));
    }
}
