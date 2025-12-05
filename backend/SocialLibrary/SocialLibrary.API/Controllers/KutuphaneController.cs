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
        var icerik = await _context.Icerikler.FindAsync(istek.IcerikId);
        if (icerik == null) return NotFound("İçerik bulunamadı.");

        Aktivite? mevcut = null;

        if (istek.Tur != "YORUM")
        {
            mevcut = await _context.Aktiviteler
                .FirstOrDefaultAsync(x => x.KullaniciId == istek.KullaniciId
                                       && x.IcerikId == istek.IcerikId
                                       && x.EylemTuru == istek.Tur);
        }

        string aciklama = GetAciklama(istek.Tur, icerik.IcerikTuru);

        if (mevcut != null)
        {
            if (istek.Puan > 0) mevcut.Puan = istek.Puan;
            mevcut.OlusturulmaTarihi = DateTime.UtcNow;
            mevcut.Aciklama = aciklama;
            if (!string.IsNullOrEmpty(istek.Yorum)) mevcut.Yorum = istek.Yorum;
        }
        else
        {
            var yeniAktivite = new Aktivite
            {
                KullaniciId = istek.KullaniciId,
                IcerikId = istek.IcerikId,
                EylemTuru = istek.Tur,
                Puan = istek.Puan,
                Yorum = istek.Yorum,
                Aciklama = aciklama,
                OlusturulmaTarihi = DateTime.UtcNow
            };
            _context.Aktiviteler.Add(yeniAktivite);
        }

        await _context.SaveChangesAsync();

        if (istek.Tur == "PUAN")
        {
            var tumPuanlar = await _context.Aktiviteler
                .Where(x => x.IcerikId == istek.IcerikId && x.EylemTuru == "PUAN" && x.Puan > 0)
                .Select(x => x.Puan)
                .ToListAsync();

            if (tumPuanlar.Any())
            {
                double yeniOrtalama = tumPuanlar.Average(x => x.Value);

                icerik.Puan = Math.Round(yeniOrtalama, 1);

                _context.Icerikler.Update(icerik);
                await _context.SaveChangesAsync();
            }
        }

        return Ok("İşlem kaydedildi.");
    }

    private string GetAciklama(string eylemTuru, string icerikTuru)
    {
        string nesne = icerikTuru == "Film" ? "bir filmi" : "bir kitabı";
        string nesne2 = icerikTuru == "Film" ? "bir film" : "bir kitap";

        return eylemTuru switch
        {
            "PUAN" => $"{nesne} oyladı.", 
            "YORUM" => $"{nesne2} hakkında yorum yaptı.", 
            "IZLENDI" => "bir filmi izledi.",
            "OKUNDU" => "bir kitabı okudu.",
            "IZLENECEK" => "bir filmi izleme listesine ekledi.",
            "OKUNACAK" => "bir kitabı okuma listesine ekledi.",
            "BEGENI" => "bir aktiviteyi beğendi.",
            _ => "bir işlem yaptı."
        };
    }

    [HttpGet("Yorumlar/{icerikId}")]
    public async Task<IActionResult> YorumlariGetir(int icerikId)
    {
        var yorumlar = await _context.Aktiviteler
            .Include(a => a.Kullanici) 
            .Where(a => a.IcerikId == icerikId && a.EylemTuru == "YORUM")
            .OrderByDescending(a => a.OlusturulmaTarihi)
            .Select(a => new
            {
                Id = a.Id,
                KullaniciId = a.KullaniciId,
                KullaniciAdi = a.Kullanici.KullaniciAdi,
                Avatar = !string.IsNullOrEmpty(a.Kullanici.ProfilResmiUrl)
                         ? a.Kullanici.ProfilResmiUrl
                         : "https://via.placeholder.com/50",
                Yorum = a.Yorum,
                Tarih = a.OlusturulmaTarihi
            })
            .ToListAsync();

        return Ok(yorumlar);
    }

    [HttpGet("Profil/{userId}")]
    public async Task<IActionResult> ProfilGetir(int userId)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(userId);
        if (kullanici == null) return NotFound();

        var aktiviteler = await _context.Aktiviteler
            .Include(a => a.Icerik)
            .Where(a => a.KullaniciId == userId)
            .OrderByDescending(a => a.OlusturulmaTarihi) 
            .ToListAsync();

        var profilVerisi = new
        {
            Kullanici = kullanici,

            SonAktiviteler = aktiviteler
                 .Where(a => a.EylemTuru == "YORUM" || a.EylemTuru == "PUAN") 
                 .Take(4) // Son 5 tanesi
                 .Select(a => new {
                     Id = a.Id,
                     IcerikId = a.Icerik.Id,
                     Baslik = a.Icerik.Baslik,
                     Tur = a.EylemTuru,
                     Zaman = a.OlusturulmaTarihi,
                     Aciklama = a.Aciklama
                 }).ToList(),
            Izlediklerim = aktiviteler.Where(x => x.EylemTuru == "IZLENDI").Select(x => x.Icerik).ToList(),
            Izlenecekler = aktiviteler.Where(x => x.EylemTuru == "IZLENECEK").Select(x => x.Icerik).ToList(),
            Okuduklarim = aktiviteler.Where(x => x.EylemTuru == "OKUNDU").Select(x => x.Icerik).ToList(),
            Okunacaklar = aktiviteler.Where(x => x.EylemTuru == "OKUNACAK").Select(x => x.Icerik).ToList()
        };
        return Ok(profilVerisi);
        
    }  

    [HttpPut("ProfilGuncelle")]
    public async Task<IActionResult> ProfilGuncelle([FromBody] KullaniciGuncellemeIstegi istek)
    {
        var kullanici = await _context.Kullanicilar.FindAsync(istek.Id);
        if (kullanici == null) return NotFound();

        kullanici.Biyografi = istek.Biyografi;
        kullanici.ProfilResmiUrl = istek.ProfilResmiUrl;

        await _context.SaveChangesAsync();
        return Ok("Profil güncellendi.");
    }
 
    [HttpDelete("YorumSil/{yorumId}")]
    public async Task<IActionResult> YorumSil(int yorumId, int kullaniciId)
    {
        var yorum = await _context.Aktiviteler.FindAsync(yorumId);
        if (yorum == null) return NotFound();

        if (yorum.KullaniciId != kullaniciId)
            return Unauthorized("Sadece kendi yorumunuzu silebilirsiniz.");

        _context.Aktiviteler.Remove(yorum);
        await _context.SaveChangesAsync();
        return Ok("Yorum silindi.");
    }

    [HttpPut("YorumGuncelle")]
    public async Task<IActionResult> YorumGuncelle([FromBody] YorumGuncelleDto istek)
    {
        var yorum = await _context.Aktiviteler.FindAsync(istek.YorumId);
        if (yorum == null) return NotFound();

        yorum.Yorum = istek.YeniMetin;
        yorum.OlusturulmaTarihi = DateTime.UtcNow; 

        await _context.SaveChangesAsync();
        return Ok("Yorum güncellendi.");
    }
}

public class KullaniciGuncellemeIstegi
{
    public int Id { get; set; }
    public string Biyografi { get; set; }
    public string ProfilResmiUrl { get; set; }
}

public class AktiviteIstegi
{
    public int KullaniciId { get; set; }
    public int IcerikId { get; set; }
    public string Tur { get; set; }
    public double? Puan { get; set; }
    public string? Yorum { get; set; } //
}