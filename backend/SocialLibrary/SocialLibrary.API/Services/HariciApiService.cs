using System.Text.Json;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Services
{
    public class HariciApiService
    {
        private readonly HttpClient _httpClient;
        private const string TmdbApiKey = "d77629dc33fc59febdd280f89da58160";

        public HariciApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<Film?> FilmDetayGetir(int tmdbId)
        {
            var url = $"https://api.themoviedb.org/3/movie/{tmdbId}?api_key={TmdbApiKey}&language=tr-TR&append_to_response=credits";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var detay = JsonSerializer.Deserialize<TmdbMovieDetail>(json);
            if (detay == null) return null;

            var yonetmen = detay.credits?.crew?.FirstOrDefault(c => c.job == "Director")?.name ?? "Bilinmiyor";
            var turler = detay.genres != null ? string.Join(", ", detay.genres.Select(g => g.name)) : "";
            var oyuncular = detay.credits?.cast != null ? string.Join(", ", detay.credits.cast.Take(5).Select(c => c.name)) : "";

            return new Film
            {
                DisKaynakId = detay.id.ToString(),
                Baslik = detay.title,
                GorselUrl = !string.IsNullOrEmpty(detay.poster_path) ? $"https://image.tmdb.org/t/p/w500{detay.poster_path}" : "https://via.placeholder.com/300x450",
                Aciklama = string.IsNullOrEmpty(detay.overview) ? "Özet bulunamadı." : detay.overview,
                YayinYili = !string.IsNullOrEmpty(detay.release_date) && detay.release_date.Length >= 4 ? detay.release_date.Substring(0, 4) : "2000",
                Puan = 0, 
                Sure = detay.runtime,
                Yonetmen = yonetmen,
                Turler = turler,
                Oyuncular = oyuncular
            };
        }

        public async Task<List<Icerik>> KaliteliFilmleriGetir()
        {
            var filmler = new List<Icerik>();

            var donemler = new List<(string Bas, string Bit)>
            {
                ("1980-01-01", "1989-12-31"),
                ("1990-01-01", "1999-12-31"),
                ("2000-01-01", "2010-12-31"),
                ("2011-01-01", "2020-12-31"),
                ("2023-01-01", "2025-12-31")
            };

            foreach (var donem in donemler)
            {
                // o dönemin en çok oy alan filmleri
                var url = $"https://api.themoviedb.org/3/discover/movie?api_key={TmdbApiKey}&language=tr-TR&sort_by=vote_count.desc&primary_release_date.gte={donem.Bas}&primary_release_date.lte={donem.Bit}&page=1";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var liste = JsonSerializer.Deserialize<TmdbResponse>(json);

                    if (liste?.results != null)
                    {
                        // her dönemden 10 film
                        foreach (var ozet in liste.results.Take(10))
                        {
                            var detayliFilm = await FilmDetayGetir(ozet.id);
                            if (detayliFilm != null) filmler.Add(detayliFilm);
                        }
                    }
                }
            }
            return filmler;
        }

        public async Task<List<Icerik>> KaliteliKitaplariGetir()
        {
            var kitaplar = new List<Icerik>();

            string[] aramalar = { "subject:history", "subject:science", "subject:fiction", "Harry Potter", "Agatha Christie" };

            foreach (var arama in aramalar)
            {
                var url = $"https://www.googleapis.com/books/v1/volumes?q={arama}&langRestrict=tr&maxResults=10&orderBy=relevance";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var veri = JsonSerializer.Deserialize<GoogleBooksResponse>(json, options);

                    if (veri?.items != null)
                    {
                        var temizVeri = veri.items
                            .Where(k => k.volumeInfo.imageLinks?.thumbnail != null)
                            .Select(k => new Kitap
                            {
                                DisKaynakId = k.id,
                                Baslik = k.volumeInfo.title,
                                GorselUrl = k.volumeInfo.imageLinks.thumbnail,
                                Aciklama = k.volumeInfo.description ?? "Özet yok.",
                                YayinYili = k.volumeInfo.publishedDate,
                                Puan = 0,
                                Yazarlar = k.volumeInfo.authors != null ? string.Join(", ", k.volumeInfo.authors) : "Bilinmiyor",
                                SayfaSayisi = k.volumeInfo.pageCount,
                                Turler = k.volumeInfo.categories != null ? string.Join(", ", k.volumeInfo.categories) : "Genel"
                            }).Cast<Icerik>().ToList();

                        kitaplar.AddRange(temizVeri);
                    }
                }
            }
            return kitaplar.GroupBy(x => x.DisKaynakId).Select(g => g.First()).Take(50).ToList();
        }
        public async Task<List<Icerik>> FilmAra(string sorgu)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={TmdbApiKey}&query={sorgu}&language=tr-TR";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Icerik>();
            var json = await response.Content.ReadAsStringAsync();
            var veri = JsonSerializer.Deserialize<TmdbResponse>(json);
            if (veri?.results == null) return new List<Icerik>();

            return veri.results.Where(f => !string.IsNullOrEmpty(f.poster_path)).Select(f => new Film
            {
                DisKaynakId = f.id.ToString(),
                Baslik = f.title,
                GorselUrl = $"https://image.tmdb.org/t/p/w500{f.poster_path}",
                Aciklama = f.overview,
                YayinYili = !string.IsNullOrEmpty(f.release_date) ? f.release_date.Substring(0, 4) : "",
                Puan = 0,
                Turler = "Film",
                Yonetmen = "Detayda Görünür"
            }).Cast<Icerik>().Take(10).ToList();
        }

        public async Task<List<Icerik>> KitapAra(string sorgu)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={sorgu}&langRestrict=tr&maxResults=10";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<Icerik>();
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var veri = JsonSerializer.Deserialize<GoogleBooksResponse>(json, options);
            if (veri?.items == null) return new List<Icerik>();

            return veri.items.Where(k => k.volumeInfo.imageLinks?.thumbnail != null).Select(k => new Kitap
            {
                DisKaynakId = k.id,
                Baslik = k.volumeInfo.title,
                GorselUrl = k.volumeInfo.imageLinks.thumbnail,
                Aciklama = k.volumeInfo.description,
                YayinYili = k.volumeInfo.publishedDate,
                Puan = 0,
                Yazarlar = k.volumeInfo.authors != null ? string.Join(", ", k.volumeInfo.authors) : "Bilinmiyor",
                SayfaSayisi = k.volumeInfo.pageCount
            }).Cast<Icerik>().ToList();
        }
    }
}
