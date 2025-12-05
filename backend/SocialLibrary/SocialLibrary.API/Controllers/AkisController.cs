using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class AkisController : ControllerBase
{
    private readonly AppDbContext _context;

    public AkisController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AkisiGetir(int kullaniciId, int sayfa = 1)
    {
        int kayitSayisi = 10;

        var takipEdilenler = await _context.Takipler
            .Where(t => t.TakipEdenKullaniciId == kullaniciId)
            .Select(t => t.TakipEdilenKullaniciId)
            .ToListAsync();
        takipEdilenler.Add(kullaniciId);

        bool globalAkis = takipEdilenler.Count <= 1;

        var sorgu = _context.Aktiviteler
            .Include(a => a.Kullanici)
            .Include(a => a.Icerik)
            .AsQueryable();

        sorgu = sorgu.Where(a => a.EylemTuru == "YORUM" || a.EylemTuru == "PUAN");

        if (!globalAkis)
        {
            sorgu = sorgu.Where(a => takipEdilenler.Contains(a.KullaniciId));
        }

        var aktiviteler = await sorgu
            .OrderByDescending(a => a.OlusturulmaTarihi)
            .Skip((sayfa - 1) * kayitSayisi)
            .Take(kayitSayisi)
            .Select(a => new
            {
                Id = a.Id,
                KullaniciId = a.KullaniciId,
                KullaniciAdi = a.Kullanici.KullaniciAdi,
                ProfilResmi = a.Kullanici.ProfilResmiUrl,
                Aciklama = a.Aciklama,
                Zaman = a.OlusturulmaTarihi,
                IcerikId = a.Icerik.Id,
                IcerikBaslik = a.Icerik.Baslik,
                IcerikGorsel = a.Icerik.GorselUrl,
                Tur = a.EylemTuru,
                Puan = a.Puan,
                YorumMetni = a.Yorum,

                BegendiMi = _context.Begeniler.Any(b => b.AktiviteId == a.Id && b.KullaniciId == kullaniciId),
                BegeniSayisi = _context.Begeniler.Count(b => b.AktiviteId == a.Id)
            })
            .ToListAsync();

        return Ok(aktiviteler);
    }
}