using Microsoft.AspNetCore.Mvc;
using SocialLibrary.API.Models;
using SocialLibrary.API.Services;

[Route("api/[controller]")]
[ApiController]
public class AramaController : ControllerBase
{
    private readonly HariciApiService _apiService;

    public AramaController(HariciApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Ara(string q)
    {
        // Arama boşsa boş liste dön
        if (string.IsNullOrWhiteSpace(q)) return Ok(new List<Icerik>());

        // Servisten verileri çek (Servis artık 'Film' ve 'Kitap' nesneleri dönüyor)
        var filmGorevi = _apiService.FilmAra(q);
        var kitapGorevi = _apiService.KitapAra(q);

        await Task.WhenAll(filmGorevi, kitapGorevi);

        // Listeleri birleştir
        var tumSonuclar = new List<Icerik>();
        tumSonuclar.AddRange(filmGorevi.Result);
        tumSonuclar.AddRange(kitapGorevi.Result);

        return Ok(tumSonuclar);
    }
}