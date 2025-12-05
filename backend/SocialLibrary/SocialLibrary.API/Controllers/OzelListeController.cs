using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class OzelListeController : ControllerBase
{
    private readonly AppDbContext _context;

    public OzelListeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("Getir/{kullaniciId}")]
    public async Task<IActionResult> ListeleriGetir(int kullaniciId)
    {
        var listeler = await _context.OzelListeler
            .Where(x => x.KullaniciId == kullaniciId)
            .OrderByDescending(x => x.OlusturulmaTarihi)
            .ToListAsync();
        return Ok(listeler);
    }

    [HttpPost("Olustur")]
    public async Task<IActionResult> ListeOlustur([FromBody] OzelListe liste)
    {
        liste.OlusturulmaTarihi = DateTime.UtcNow;
        _context.OzelListeler.Add(liste);
        await _context.SaveChangesAsync();
        return Ok(liste);
    }

    [HttpPost("IcerikEkle")]
    public async Task<IActionResult> IcerikEkle([FromBody] OzelListeOgesi oge)
    {
        bool varMi = await _context.OzelListeOgeleri
            .AnyAsync(x => x.OzelListeId == oge.OzelListeId && x.IcerikId == oge.IcerikId);

        if (varMi) return BadRequest("Bu içerik zaten listede var.");

        _context.OzelListeOgeleri.Add(oge);
        await _context.SaveChangesAsync();
        return Ok("Listeye eklendi.");
    }

    [HttpGet("Detay/{listeId}")]
    public async Task<IActionResult> ListeDetayi(int listeId)
    {
        var listeOgeleri = await _context.OzelListeOgeleri
            .Where(x => x.OzelListeId == listeId)
            .Include(x => x.Icerik) 
            .ToListAsync();

        var icerikler = listeOgeleri
            .Where(x => x.Icerik != null)
            .Select(x => x.Icerik)
            .ToList();

        return Ok(icerikler);
    }
}