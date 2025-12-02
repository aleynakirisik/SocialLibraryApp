using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class KutuphaneController : ControllerBase
{
    private readonly AppDbContext _context;

    public KutuphaneController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("Ekle")]
    public async Task<IActionResult> ListeyeEkle([FromBody] AktiviteIstegi istek)
    {
        // Yorum ve Puanlamalar her seferinde yeni bir aktivite olarak eklenir (Feed akışı için)
        // Ancak Listeye Ekleme (İzlenecek) durumunda güncelleme yapılabilir.

        var yeniAktivite = new Aktivite
        {
            KullaniciId = istek.KullaniciId,
            IcerikId = istek.IcerikId,
            EylemTuru = istek.Tur,
            Puan = istek.Puan,
            Yorum = istek.Yorum,
            Aciklama = GetAciklama(istek.Tur, istek.Puan),
            OlusturulmaTarihi = DateTime.UtcNow
        };

        _context.Aktiviteler.Add(yeniAktivite);
        await _context.SaveChangesAsync();
        return Ok("İşlem kaydedildi.");
    }

    // 2. BİR İÇERİĞE AİT YORUMLARI GETİR (Detay Sayfası İçin)
    [HttpGet("Yorumlar/{icerikId}")]
    public async Task<IActionResult> YorumlariGetir(int icerikId)
    {
        var yorumlar = await _context.Aktiviteler
            .Include(a => a.Kullanici) // Yorumu yapanın adını/resmini de al
            .Where(a => a.IcerikId == icerikId && a.EylemTuru == "YORUM")
            .OrderByDescending(a => a.OlusturulmaTarihi)
            .Select(a => new
            {
                Id = a.Id,
                KullaniciAdi = a.Kullanici.KullaniciAdi,
                Avatar = a.Kullanici.ProfilResmiUrl,
                Yorum = a.Yorum,
                Tarih = a.OlusturulmaTarihi
            })
            .ToListAsync();

        return Ok(yorumlar);
    }

    // 2. PROFİL BİLGİLERİNİ GETİR (PDF Madde 2.1.5 - Kütüphanem)
    [HttpGet("Profil/{userId}")]
    public async Task<IActionResult> ProfilGetir(int userId)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(userId);
        if (kullanici == null) return NotFound();

        // Kullanıcının tüm hareketlerini çek
        var aktiviteler = await _context.Aktiviteler
            .Include(a => a.Icerik) // İçerik detaylarını da getir (Resim, Başlık)
            .Where(a => a.KullaniciId == userId)
            .ToListAsync();

        // Frontend için temiz bir obje oluştur
        var profilVerisi = new
        {
            Kullanici = kullanici,
            Izlediklerim = aktiviteler.Where(x => x.EylemTuru == "IZLENDI").Select(x => x.Icerik).ToList(),
            Izlenecekler = aktiviteler.Where(x => x.EylemTuru == "IZLENECEK").Select(x => x.Icerik).ToList(),
            Okuduklarim = aktiviteler.Where(x => x.EylemTuru == "OKUNDU").Select(x => x.Icerik).ToList(),
            Okunacaklar = aktiviteler.Where(x => x.EylemTuru == "OKUNACAK").Select(x => x.Icerik).ToList()
        };

        return Ok(profilVerisi);
    }

    // Yardımcı Metot: Feed'de görünecek yazı
    private string GetAciklama(string tur, double? puan)
    {
        return tur switch
        {
            "YORUM" => "bir yorum yaptı.",
            "PUAN" => $"bir içeriğe {puan}/10 puan verdi.",
            "IZLENDI" => "bir filmi izledi.",
            "IZLENECEK" => "bir filmi izleme listesine ekledi.",
            "OKUNDU" => "bir kitabı okudu.",
            "OKUNACAK" => "bir kitabı okuma listesine ekledi.",
            _ => "bir işlem yaptı."
        };
    }
}

// Frontend'den gelecek veri modeli
public class AktiviteIstegi
{
    public int KullaniciId { get; set; }
    public int IcerikId { get; set; }
    public string Tur { get; set; }
    public double? Puan { get; set; }
    public string? Yorum { get; set; } //
}