using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class SosyalController : ControllerBase
{
    private readonly AppDbContext _context;

    public SosyalController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("TakipIslemi")]
    public async Task<IActionResult> TakipEt([FromBody] TakipIstegi istek)
    {
        var mevcut = await _context.Takipler
            .FirstOrDefaultAsync(x => x.TakipEdenKullaniciId == istek.BenimId && x.TakipEdilenKullaniciId == istek.BaskasiId);

        if (mevcut != null)
        {
            _context.Takipler.Remove(mevcut);
            await _context.SaveChangesAsync();
            return Ok("Takipten çıkıldı");
        }
        else
        {
            var yeniTakip = new Takip { TakipEdenKullaniciId = istek.BenimId, TakipEdilenKullaniciId = istek.BaskasiId };
            _context.Takipler.Add(yeniTakip);
            await _context.SaveChangesAsync();
            return Ok("Takip edildi");
        }
    }

    [HttpGet("Durum")]
    public async Task<IActionResult> TakipDurumu(int ben, int o)
    {
        var takipEdiyorMu = await _context.Takipler.AnyAsync(x => x.TakipEdenKullaniciId == ben && x.TakipEdilenKullaniciId == o);
        return Ok(takipEdiyorMu);
    }

    [HttpGet("Filtrele")]
    public async Task<IActionResult> Filtrele(string? tur, string? yil, double? minPuan)
    {
        var sorgu = _context.Icerikler.AsQueryable();

        if (!string.IsNullOrEmpty(tur) && tur != "Hepsi")
            sorgu = sorgu.Where(x => x.IcerikTuru == tur); 

        if (!string.IsNullOrEmpty(yil))
            sorgu = sorgu.Where(x => x.YayinYili.Contains(yil));

        if (minPuan.HasValue)
            sorgu = sorgu.Where(x => x.Puan >= minPuan);

        var sonuclar = await sorgu.OrderByDescending(x => x.Puan).Take(20).ToListAsync();
        return Ok(sonuclar);
    }

    [HttpPost("Begen")]
    public async Task<IActionResult> Begen([FromBody] BegeniIstegi istek)
    {
        var mevcut = await _context.Begeniler
            .FirstOrDefaultAsync(x => x.KullaniciId == istek.KullaniciId && x.AktiviteId == istek.AktiviteId);

        if (mevcut != null)
        {
            _context.Begeniler.Remove(mevcut); 
            await _context.SaveChangesAsync();
            return Ok(false); 
        }
        else
        {
            var yeniBegeni = new Begeni { KullaniciId = istek.KullaniciId, AktiviteId = istek.AktiviteId };
            _context.Begeniler.Add(yeniBegeni);
            await _context.SaveChangesAsync();
            return Ok(true);
        }
    }

    [HttpPost("AktiviteYorumEkle")]
    public async Task<IActionResult> AktiviteYorumEkle([FromBody] AktiviteYorumuIstegi istek)
    {
        var yeniYorum = new AktiviteYorumu
        {
            AktiviteId = istek.AktiviteId,
            KullaniciId = istek.KullaniciId,
            Yorum = istek.Yorum,
            Tarih = DateTime.UtcNow
        };

        _context.AktiviteYorumlari.Add(yeniYorum);
        await _context.SaveChangesAsync();

        var detayliYorum = await _context.AktiviteYorumlari
            .Include(x => x.Kullanici)
            .FirstOrDefaultAsync(x => x.Id == yeniYorum.Id);

        return Ok(new
        {
            Id = detayliYorum.Id,
            KullaniciAdi = detayliYorum.Kullanici.KullaniciAdi,
            ProfilResmi = detayliYorum.Kullanici.ProfilResmiUrl,
            Yorum = detayliYorum.Yorum,
            Tarih = detayliYorum.Tarih
        });
    }

    [HttpGet("AktiviteYorumlari/{aktiviteId}")]
    public async Task<IActionResult> AktiviteYorumlariGetir(int aktiviteId)
    {
        var yorumlar = await _context.AktiviteYorumlari
            .Where(x => x.AktiviteId == aktiviteId)
            .Include(x => x.Kullanici)
            .OrderBy(x => x.Tarih)
            .Select(x => new {
                Id = x.Id,
                KullaniciAdi = x.Kullanici.KullaniciAdi,
                ProfilResmi = x.Kullanici.ProfilResmiUrl,
                Yorum = x.Yorum,
                Tarih = x.Tarih
            })
            .ToListAsync();

        return Ok(yorumlar);
    }
}

public class AktiviteYorumuIstegi
{
    public int AktiviteId { get; set; }
    public int KullaniciId { get; set; }
    public string Yorum { get; set; }
}


public class BegeniIstegi { public int KullaniciId { get; set; } public int AktiviteId { get; set; } }


public class TakipIstegi { public int BenimId { get; set; } public int BaskasiId { get; set; } }