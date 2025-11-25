using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;

namespace SocialLibrary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AkisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AkisController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/akis
        [HttpGet]
        public async Task<IActionResult> AkisiGetir()
        {
            var aktiviteler = await _context.Aktiviteler
                .Include(a => a.Kullanici)
                .Include(a => a.Icerik)
                .OrderByDescending(a => a.OlusturulmaTarihi) // En yeniler en üstte
                .Take(20) // Son 20 aktivite
                .Select(a => new
                {
                    Id = a.Id,
                    KullaniciAdi = a.Kullanici.KullaniciAdi,
                    ProfilResmi = a.Kullanici.ProfilResmiUrl,
                    Aciklama = a.Aciklama,
                    Zaman = a.OlusturulmaTarihi,
                    IcerikBaslik = a.Icerik.Baslik,
                    IcerikGorsel = a.Icerik.GorselUrl,
                    Tur = a.EylemTuru,
                    Puan = a.Puan,
                    YorumMetni = a.Yorum
                })
                .ToListAsync();

            return Ok(aktiviteler);
        }
    }
}
