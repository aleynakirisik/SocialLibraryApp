using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public AuthController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost("Kayit")]
    public async Task<IActionResult> Kayit([FromBody] KayitDto istek)
    {

        if (await _context.Kullanicilar.AnyAsync(x => x.Email == istek.Email))
        {
            return BadRequest("Bu e-posta adresi zaten kullanımda.");
        }

        var yeniKullanici = new Kullanici
        {
            KullaniciAdi = istek.KullaniciAdi,
            Email = istek.Email,
            Sifre = istek.Sifre, 
            Biyografi = "Merhaba, ben yeni bir üyeyim!",
            ProfilResmiUrl = "https://via.placeholder.com/150"
        };

        _context.Kullanicilar.Add(yeniKullanici);
        await _context.SaveChangesAsync();

        return Ok("Kayıt başarılı! Giriş yapabilirsiniz.");
    }

    [HttpPost("Giris")]
    public async Task<IActionResult> Giris([FromBody] GirisDto istek)
    {
        var kullanici = await _context.Kullanicilar
            .FirstOrDefaultAsync(x => x.Email == istek.Email);

        if (kullanici == null)
        {
            Console.WriteLine($"[HATA] Bu email ('{istek.Email}') veritabanında YOK.");
            return BadRequest("E-posta adresi hatalı.");
        }

        if (kullanici.Sifre != istek.Sifre)
        {
            return BadRequest("Şifre hatalı.");
        }

        return Ok(new
        {
            Id = kullanici.Id,
            KullaniciAdi = kullanici.KullaniciAdi,
            Email = kullanici.Email,
            ProfilResmi = kullanici.ProfilResmiUrl
        });
    }

    [HttpPost("SifremiUnuttum")]
    public async Task<IActionResult> SifremiUnuttum([FromBody] SifreUnuttumDto istek)
    {
        var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.Email == istek.Email);

        if (kullanici == null)
            return BadRequest("Bu e-posta adresine kayıtlı kullanıcı bulunamadı.");

        string link = $"http://localhost:3000/sifre-sifirla?email={istek.Email}";

        string mailIcerigi = $@"
            <h3>Şifre Sıfırlama İsteği</h3>
            <p>Merhaba {kullanici.KullaniciAdi},</p>
            <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
            <a href='{link}'>Şifremi Sıfırla</a>
            <p>Eğer bu isteği siz yapmadıysanız, dikkate almayınız.</p>
        ";

        try
        {
            await _emailService.Gonder(istek.Email, "Şifre Sıfırlama", mailIcerigi);
            return Ok($"Sıfırlama bağlantısı {istek.Email} adresine başarıyla gönderildi.");
        }
        catch (Exception ex)
        {
            return BadRequest("Mail gönderilirken hata oluştu: " + ex.Message);
        }
    }

    [HttpPost("SifreSifirla")]
    public async Task<IActionResult> SifreSifirla([FromBody] SifreSifirlaDto istek)
    {
        var kullanici = await _context.Kullanicilar.FirstOrDefaultAsync(x => x.Email == istek.Email);

        if (kullanici == null)
            return BadRequest("Kullanıcı bulunamadı.");

        kullanici.Sifre = istek.YeniSifre.Trim();

        _context.Kullanicilar.Update(kullanici); 
        await _context.SaveChangesAsync();

        Console.WriteLine($"[BİLGİ] {kullanici.Email} şifresini değiştirdi. Yeni şifre: {kullanici.Sifre}");

        return Ok("Şifreniz başarıyla güncellendi. Giriş yapabilirsiniz.");
    }
}