using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Data;
using SocialLibrary.API.Models;

[Route("api/[controller]")]
[ApiController]
public class IcerikController : ControllerBase
{
    private readonly AppDbContext _context;

    public IcerikController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Icerik/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetIcerik(int id)
    {
        // TPT kullandığımız için Icerikler tablosundan sorgulayınca
        // EF Core otomatik olarak Film veya Kitap tablolarıyla birleştirip getirir.
        var icerik = await _context.Icerikler
            .FirstOrDefaultAsync(x => x.Id == id);

        if (icerik == null)
        {
            return NotFound("İçerik bulunamadı.");
        }

        return Ok(icerik);
    }
}