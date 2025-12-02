using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // 1. Veritabanını oluştur (Yoksa)
        await _context.Database.EnsureCreatedAsync();

        // 2. TEMİZLİK ZAMANI: Önce eski/hatalı/çift verileri SİLİYORUZ.
        // Böylece her çalıştırdığında veritabanın sıfırlanır ve temizlenir.
        if (_context.Icerikler.Any())
        {
            _context.Icerikler.RemoveRange(_context.Icerikler);
            await _context.SaveChangesAsync();
        }

        // 3. Admin Kullanıcısı Yoksa Oluştur
        if (!_context.Kullanicilar.Any())
        {
            var admin = new Kullanici
            {
                KullaniciAdi = "admin",
                Email = "admin@gmail.com",
                Sifre = "12345",
                Biyografi = "Sistem Yöneticisi",
                ProfilResmiUrl = "https://ui-avatars.com/api/?name=Admin&background=000&color=fff"
            };
            _context.Kullanicilar.Add(admin);
        }

        // 4. Kaliteli İçerikleri API'den Çek
        var eklenecekler = new List<Icerik>();

        // 50 Film
        var filmler = await _apiService.KaliteliFilmleriGetir();
        eklenecekler.AddRange(filmler);

        // 50 Kitap
        var kitaplar = await _apiService.KaliteliKitaplariGetir();
        eklenecekler.AddRange(kitaplar);

        // 5. Veritabanına Kaydet (Distinct ile ID kontrolü yaparak garantiye alıyoruz)
        var temizListe = eklenecekler
            .GroupBy(x => x.DisKaynakId) // Aynı ID'li olanları grupla
            .Select(g => g.First())      // Sadece ilkini al
            .ToList();

        _context.Icerikler.AddRange(temizListe);
        await _context.SaveChangesAsync();

        return Ok($"Veritabanı temizlendi ve {temizListe.Count} adet kaliteli içerik yüklendi.");
    }
}