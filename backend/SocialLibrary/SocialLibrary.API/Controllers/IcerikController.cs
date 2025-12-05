using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class IcerikController : ControllerBase
{
    private readonly AppDbContext _context;

    public IcerikController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIcerik(int id, int kullaniciId = 0)
    {
        var icerik = await _context.Icerikler.FirstOrDefaultAsync(x => x.Id == id);
        if (icerik == null) return NotFound("İçerik bulunamadı.");

        // platform puanı hesaplama
        var puanlar = await _context.Aktiviteler
            .Where(x => x.IcerikId == id && x.EylemTuru == "PUAN" && x.Puan > 0)
            .Select(x => x.Puan)
            .ToListAsync();

        double ortalama = 0;
        if (puanlar.Count > 0)
        {
            ortalama = puanlar.Average(x => x.Value);
        }

        var benimAktivitem = await _context.Aktiviteler
            .FirstOrDefaultAsync(x => x.IcerikId == id && x.KullaniciId == kullaniciId && x.EylemTuru.Contains("PUAN"));

        var listemdeMi = await _context.Aktiviteler
            .AnyAsync(x => x.IcerikId == id && x.KullaniciId == kullaniciId && (x.EylemTuru == "IZLENECEK" || x.EylemTuru == "OKUNACAK"));

        var sonuc = new IcerikDetayDto
        {
            Icerik = icerik,
            PlatformPuani = Math.Round(ortalama, 1),
            ToplamOy = puanlar.Count,
            ListemdeMi = listemdeMi,
            KullaniciPuani = benimAktivitem?.Puan?.ToString() ?? "0"
        };

        return Ok(sonuc);
    }
}