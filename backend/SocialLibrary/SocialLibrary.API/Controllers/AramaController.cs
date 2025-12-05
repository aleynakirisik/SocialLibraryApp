using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;

[Route("api/[controller]")]
[ApiController]
public class AramaController : ControllerBase
{
    private readonly HariciApiService _apiService;
    private readonly AppDbContext _context;

    public AramaController(HariciApiService apiService, AppDbContext context)
    {
        _apiService = apiService;
        _context = context;
    }

    [HttpGet("Ara")]
    public async Task<IActionResult> Ara(string? q, string? tur, string? yil, double? minPuan)
    {
        if (string.IsNullOrWhiteSpace(q) &&
            (string.IsNullOrWhiteSpace(tur) || tur == "Hepsi") &&
            string.IsNullOrWhiteSpace(yil) &&
            (minPuan == null || minPuan == 0))
        {
            return Ok(new List<Icerik>());
        }

        var sorgu = _context.Icerikler.AsQueryable();

        //kelime arama
        if (!string.IsNullOrWhiteSpace(q))
        {
            sorgu = sorgu.Where(x => x.Baslik.ToLower().Contains(q.ToLower()));
        }

        // tür
        if (!string.IsNullOrWhiteSpace(tur) && tur != "Hepsi")
        {
            sorgu = sorgu.Where(x => x.IcerikTuru == tur);
        }

        // yıl
        if (!string.IsNullOrWhiteSpace(yil))
        {
            sorgu = sorgu.Where(x => x.YayinYili != null && x.YayinYili.Contains(yil));
        }

        // puan
        if (minPuan.HasValue && minPuan > 0)
        {
            sorgu = sorgu.Where(x => x.Puan >= minPuan);
        }

        var sonuclar = await sorgu
            .OrderByDescending(x => x.Puan)
            .Take(50)
            .ToListAsync();

        return Ok(sonuclar);
    }

    [HttpGet("Vitrin")]
    public async Task<IActionResult> VitrinGetir()
    {
        var enCokEtkilesimAlanlar = await _context.Aktiviteler
            .GroupBy(a => a.IcerikId)
            .OrderByDescending(g => g.Count()) 
            .Select(g => g.Key)
            .Take(10)
            .ToListAsync();

        var enPopulerler = await _context.Icerikler
            .Where(x => enCokEtkilesimAlanlar.Contains(x.Id))
            .ToListAsync();

        var enYuksekPuanliIdler = await _context.Aktiviteler
            .Where(a => a.EylemTuru == "PUAN" && a.Puan > 0)
            .GroupBy(a => a.IcerikId)
            .Select(g => new { Id = g.Key, Ortalama = g.Average(x => x.Puan) })
            .OrderByDescending(x => x.Ortalama) 
            .Select(x => x.Id)
            .Take(10)
            .ToListAsync();

        var enYuksekPuanlilar = await _context.Icerikler
            .Where(x => enYuksekPuanliIdler.Contains(x.Id))
            .ToListAsync();

        return Ok(new { EnYuksekPuan = enYuksekPuanlilar, EnPopuler = enPopulerler });
    }
}