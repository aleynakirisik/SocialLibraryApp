using Microsoft.AspNetCore.Mvc;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;

[Route("api/[controller]")]
[ApiController]
public class VeriYukleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HariciApiService _apiService;

    public VeriYukleController(AppDbContext context, HariciApiService apiService)
    {
        _context = context;
        _apiService = apiService;
    }

    [HttpPost]
    public async Task<IActionResult> VerileriDoldur()
    {
        // 1. Veritabanını oluştur
        await _context.Database.EnsureCreatedAsync();
        string mesaj = "Veritabanı kontrol edildi. ";

        // 2. Admin Kullanıcısı Oluştur
        if (!_context.Kullanicilar.Any())
        {
            var admin = new Kullanici
            {
                KullaniciAdi = "admin",
                Email = "admin@gmail.com",
                Sifre = "12345",
                Biyografi = "Yönetici hesabı",
                ProfilResmiUrl = "https://ui-avatars.com/api/?name=Admin&background=000&color=fff"
            };
            _context.Kullanicilar.Add(admin);
            await _context.SaveChangesAsync();
            mesaj += "Admin oluşturuldu. ";
        }

        // 3. İçerikleri API'den Çek ve Doldur
        if (!_context.Icerikler.Any())
        {
            var eklenecekler = new List<Icerik>();

            // Popüler Filmleri Çek
            var populerFilmler = await _apiService.PopulerFilmleriGetir();
            eklenecekler.AddRange(populerFilmler);

            // Ekstra Kitaplar
            var kitaplar = await _apiService.KitapAra("Harry Potter");
            eklenecekler.AddRange(kitaplar);

            // Hata almamak için ID'ye göre tekilleştir (Aynı içerik iki kere eklenmesin)
            var temizListe = eklenecekler
                .GroupBy(x => x.DisKaynakId)
                .Select(g => g.First())
                .ToList();

            _context.Icerikler.AddRange(temizListe);
            await _context.SaveChangesAsync();

            mesaj += $"{temizListe.Count} adet içerik yüklendi.";
        }
        else
        {
            mesaj += "İçerikler zaten dolu.";
        }

        return Ok(mesaj);
    }
}