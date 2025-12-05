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
        // veritabanı oluşturma
        await _context.Database.EnsureCreatedAsync();
        string mesaj = "";

        // admin ekle
        var adminVarMi = await _context.Kullanicilar.AnyAsync(x => x.Email == "admin@gmail.com");
        if (!adminVarMi)
        {
            var admin = new Kullanici
            {
                KullaniciAdi = "admin",
                Email = "admin@gmail.com",
                Sifre = "12345",
                Biyografi = "Film ve kitap tutkunu.",
                ProfilResmiUrl = "https://ui-avatars.com/api/?name=Admin&background=000&color=fff"
            };
            _context.Kullanicilar.Add(admin);
            await _context.SaveChangesAsync();
            mesaj += "Admin kullanıcısı oluşturuldu. ";
        }
        else
        {
            mesaj += "Kullanıcılar korundu. ";
        }
        if (!_context.Takipler.Any())
        {
            var adminUser = await _context.Kullanicilar.FirstOrDefaultAsync(u => u.KullaniciAdi == "admin");
            var digerleri = await _context.Kullanicilar.Where(u => u.KullaniciAdi != "admin").ToListAsync();

            if (adminUser != null)
            {
                foreach (var user in digerleri)
                {
                    _context.Takipler.Add(new Takip
                    {
                        TakipEdenKullaniciId = adminUser.Id,
                        TakipEdilenKullaniciId = user.Id
                    });
                }
                await _context.SaveChangesAsync();
                mesaj += "Admin diğer kullanıcıları takip etti. ";
            }
        }

        var mevcutIdler = await _context.Icerikler.Select(x => x.DisKaynakId).ToListAsync();
        var eklenecekler = new List<Icerik>();

        // TMDb
        var filmler = await _apiService.KaliteliFilmleriGetir();
        //olmayanları listeye al
        foreach (var film in filmler)
        {
            if (!mevcutIdler.Contains(film.DisKaynakId)) eklenecekler.Add(film);
        }

        // Google Books
        var kitaplar = await _apiService.KaliteliKitaplariGetir();
        //olmayanları listeye al
        foreach (var kitap in kitaplar)
        {
            if (!mevcutIdler.Contains(kitap.DisKaynakId)) eklenecekler.Add(kitap);
        }

        if (eklenecekler.Count > 0)
        {
            var temizListe = eklenecekler
                .GroupBy(x => x.DisKaynakId)
                .Select(g => g.First())
                .ToList();

            _context.Icerikler.AddRange(temizListe);
            await _context.SaveChangesAsync();
            mesaj += $"{temizListe.Count} yeni içerik veritabanına eklendi.";
        }
        else
        {
            mesaj += "Yeni içerik bulunamadı veya hepsi zaten yüklü.";
        }

        return Ok(mesaj);
    }
}