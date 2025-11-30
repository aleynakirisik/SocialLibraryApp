using System.Text.Json;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Services
{
    public class HariciApiService
    {
        private readonly HttpClient _httpClient;
        // TMDb API Key'ini buraya yapıştır:
        private const string TmdbApiKey = "d77629dc33fc59febdd280f89da58160";

        public HariciApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // FİLM ARA
        public async Task<List<Icerik>> FilmAra(string sorgu)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={TmdbApiKey}&query={sorgu}&language=tr-TR";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Icerik>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var veri = JsonSerializer.Deserialize<TmdbResponse>(jsonString);
            if (veri?.results == null) return new List<Icerik>();

            // Dikkat: "new Film" oluşturuyoruz
            return veri.results.Select(f => new Film
            {
                DisKaynakId = f.id.ToString(),
                Baslik = f.title,
                GorselUrl = !string.IsNullOrEmpty(f.poster_path) ? $"https://image.tmdb.org/t/p/w500{f.poster_path}" : "",
                Aciklama = f.overview,
                YayinYili = !string.IsNullOrEmpty(f.release_date) && f.release_date.Length >= 4 ? f.release_date.Substring(0, 4) : "",
                Puan = f.vote_average,
                // Film Özel:
                Turler = "Film", // Detaydan çekilirse doldurulur
                Yonetmen = "Bilinmiyor"
            }).Cast<Icerik>().ToList();
        }

        // POPÜLER FİLMLERİ GETİR
        public async Task<List<Icerik>> PopulerFilmleriGetir()
        {
            var url = $"https://api.themoviedb.org/3/movie/popular?api_key={TmdbApiKey}&language=tr-TR&page=1";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Icerik>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var veri = JsonSerializer.Deserialize<TmdbResponse>(jsonString);
            if (veri?.results == null) return new List<Icerik>();

            return veri.results.Select(f => new Film
            {
                DisKaynakId = f.id.ToString(),
                Baslik = f.title,
                GorselUrl = !string.IsNullOrEmpty(f.poster_path) ? $"https://image.tmdb.org/t/p/w500{f.poster_path}" : "",
                Aciklama = f.overview,
                YayinYili = !string.IsNullOrEmpty(f.release_date) ? f.release_date.Substring(0, 4) : "",
                Puan = f.vote_average,
                Yonetmen = "TMDb Popüler"
            }).Cast<Icerik>().ToList();
        }

        // KİTAP ARA
        public async Task<List<Icerik>> KitapAra(string sorgu)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={sorgu}&langRestrict=tr&maxResults=10";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Icerik>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var veri = JsonSerializer.Deserialize<GoogleBooksResponse>(jsonString, options);
            if (veri?.items == null) return new List<Icerik>();

            // Dikkat: "new Kitap" oluşturuyoruz
            return veri.items.Select(k => new Kitap
            {
                DisKaynakId = k.id,
                Baslik = k.volumeInfo.title,
                GorselUrl = k.volumeInfo.imageLinks?.thumbnail ?? "",
                Aciklama = k.volumeInfo.description,
                YayinYili = k.volumeInfo.publishedDate,
                Puan = k.volumeInfo.averageRating,
                // Kitap Özel:
                Yazarlar = k.volumeInfo.authors != null ? string.Join(", ", k.volumeInfo.authors) : "Bilinmiyor",
                SayfaSayisi = k.volumeInfo.pageCount
            }).Cast<Icerik>().ToList();
        }
    }
}