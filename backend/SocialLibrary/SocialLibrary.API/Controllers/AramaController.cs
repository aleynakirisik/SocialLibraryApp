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

    [HttpGet]
    public async Task<IActionResult> Ara(string? q)
    {
        // 1. Arama kutusu BOŞSA -> Veritabanındaki hazır vitrini getir
        if (string.IsNullOrWhiteSpace(q))
        {
            var vitrin = await _context.Icerikler
                                       .OrderByDescending(x => x.Puan)
                                       .Take(50)
                                       .ToListAsync();
            return Ok(vitrin);
        }

        // 2. Arama yapıldıysa -> API'den verileri çek
        var filmGorevi = _apiService.FilmAra(q);
        var kitapGorevi = _apiService.KitapAra(q);

        await Task.WhenAll(filmGorevi, kitapGorevi);

        var hamSonuclar = new List<Icerik>();
        hamSonuclar.AddRange(filmGorevi.Result);
        hamSonuclar.AddRange(kitapGorevi.Result);

        // 3. KRİTİK DÜZELTME: Gelenleri Veritabanına Kaydet (ID Oluşsun Diye)
        var sonuclar = new List<Icerik>();

        foreach (var item in hamSonuclar)
        {
            // Bu içerik zaten veritabanında var mı? (DisKaynakId ile kontrol et)
            var mevcut = await _context.Icerikler
                .FirstOrDefaultAsync(x => x.DisKaynakId == item.DisKaynakId);

            if (mevcut != null)
            {
                // Varsa onu kullan (Çünkü onun gerçek bir ID'si var)
                sonuclar.Add(mevcut);
            }
            else
            {
                // Yoksa veritabanına ekle
                _context.Icerikler.Add(item);
                await _context.SaveChangesAsync(); // Kaydet ki ID oluşsun
                sonuclar.Add(item); // Artık item.Id doldu (örn: 105)
            }
        }

        return Ok(sonuclar);
    }
}