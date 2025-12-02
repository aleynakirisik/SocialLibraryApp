using System.Text.Json;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Services
{
    public class HariciApiService
    {
        private readonly HttpClient _httpClient;
        // Verdiğin API Key buraya eklendi
        private const string TmdbApiKey = "d77629dc33fc59febdd280f89da58160";

        public HariciApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // --- 1. FİLM ARA (Kullanıcı arama kutusuna yazınca çalışır) ---
        // --- 1. FİLM ARA (FİLTRELİ) ---
        public async Task<List<Icerik>> FilmAra(string sorgu)
        {
            var url = $"https://api.themoviedb.org/3/search/movie?api_key={TmdbApiKey}&query={sorgu}&language=tr-TR";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return new List<Icerik>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var veri = JsonSerializer.Deserialize<TmdbResponse>(jsonString);

            if (veri?.results == null) return new List<Icerik>();

            // FİLTRELEME BURADA YAPILIYOR:
            return veri.results
                .Where(f => !string.IsNullOrEmpty(f.poster_path)) // Resmi olmayanları at
                .Where(f => f.vote_count > 10) // Hiç oy almamış, bilinmeyen filmleri at
                .Select(f => new Film
                {
                    DisKaynakId = f.id.ToString(),
                    Baslik = f.title,
                    GorselUrl = $"https://image.tmdb.org/t/p/w500{f.poster_path}",
                    Aciklama = f.overview,
                    YayinYili = !string.IsNullOrEmpty(f.release_date) && f.release_date.Length >= 4 ? f.release_date.Substring(0, 4) : "",
                    Puan = f.vote_average,
                    Turler = "Film",
                    Yonetmen = "Bilinmiyor"
                })
                .Cast<Icerik>()
                .Take(10) // En kaliteli 10 taneyi al
                .ToList();
        }

        // --- 2. KİTAP ARA (FİLTRELİ) ---
        public async Task<List<Icerik>> KitapAra(string sorgu)
        {
            var url = $"https://www.googleapis.com/books/v1/volumes?q={sorgu}&langRestrict=tr&maxResults=20"; // 20 tane iste, süzünce 10 kalsın
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return new List<Icerik>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var veri = JsonSerializer.Deserialize<GoogleBooksResponse>(jsonString, options);

            if (veri?.items == null) return new List<Icerik>();

            return veri.items
                .Where(k => k.volumeInfo.imageLinks?.thumbnail != null) // Resmi olmayanları at
                .Select(k => new Kitap
                {
                    DisKaynakId = k.id,
                    Baslik = k.volumeInfo.title,
                    GorselUrl = k.volumeInfo.imageLinks.thumbnail,
                    Aciklama = k.volumeInfo.description,
                    YayinYili = k.volumeInfo.publishedDate,
                    Puan = k.volumeInfo.averageRating,
                    Yazarlar = k.volumeInfo.authors != null ? string.Join(", ", k.volumeInfo.authors) : "Bilinmiyor",
                    SayfaSayisi = k.volumeInfo.pageCount
                })
                .Cast<Icerik>()
                .Take(10)
                .ToList();
        }

        // --- 3. 50 TANE KALİTELİ FİLM GETİR (Otomatik Yükleme) ---
        public async Task<List<Icerik>> KaliteliFilmleriGetir()
        {
            var filmler = new List<Icerik>();

            // 3 Sayfa dolaş (60 film)
            for (int sayfa = 1; sayfa <= 3; sayfa++)
            {
                var url = $"https://api.themoviedb.org/3/movie/popular?api_key={TmdbApiKey}&language=tr-TR&page={sayfa}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var veri = JsonSerializer.Deserialize<TmdbResponse>(jsonString);

                    if (veri?.results != null)
                    {
                        var sayfaVerisi = veri.results.Select(f => new Film
                        {
                            DisKaynakId = f.id.ToString(),
                            Baslik = f.title,
                            GorselUrl = !string.IsNullOrEmpty(f.poster_path) ? $"https://image.tmdb.org/t/p/w500{f.poster_path}" : "https://via.placeholder.com/300x450?text=Resim+Yok",
                            Aciklama = string.IsNullOrEmpty(f.overview) ? "Özet bilgisi henüz eklenmemiş." : f.overview,
                            YayinYili = !string.IsNullOrEmpty(f.release_date) && f.release_date.Length >= 4 ? f.release_date.Substring(0, 4) : "",
                            Puan = f.vote_average,
                            Turler = "Film",
                            Yonetmen = "Popüler Yapım"
                        }).Cast<Icerik>().ToList();

                        filmler.AddRange(sayfaVerisi);
                    }
                }
            }
            return filmler.Take(50).ToList();
        }

        // --- 4. 50 TANE KALİTELİ KİTAP GETİR (Otomatik Yükleme) ---
        public async Task<List<Icerik>> KaliteliKitaplariGetir()
        {
            var kitaplar = new List<Icerik>();

            // DÜZELTME: Yazar adı yerine direkt meşhur kitap isimlerini yazıyoruz.
            // Böylece başlıkta "Agatha Christie" yazan biyografiler yerine romanların kendisi gelecek.
            string[] aramalar = {
                "Harry Potter ve Felsefe Taşı",
                "Yüzüklerin Efendisi",
                "1984 George Orwell",
                "Hobbit",
                "Doğu Ekspresinde Cinayet", // Agatha Christie yerine kitabını yazdık
                "Dune",
                "Sefiller",
                "Suç ve Ceza",
                "Simyacı",
                "Hayvan Çiftliği"
            };

            foreach (var arama in aramalar)
            {
                var url = $"https://www.googleapis.com/books/v1/volumes?q={arama}&langRestrict=tr&maxResults=5&orderBy=relevance";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var veri = JsonSerializer.Deserialize<GoogleBooksResponse>(jsonString, options);

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
                                Puan = k.volumeInfo.averageRating ?? 0,
                                Yazarlar = k.volumeInfo.authors != null ? string.Join(", ", k.volumeInfo.authors) : "Bilinmiyor",
                                SayfaSayisi = k.volumeInfo.pageCount
                            }).Cast<Icerik>().ToList();

                        kitaplar.AddRange(temizVeri);
                    }
                }
            }

            // Çift kayıtları engelle (Aynı ID'lileri sil)
            return kitaplar
                .GroupBy(x => x.DisKaynakId)
                .Select(g => g.First())
                .Take(50)
                .ToList();
        }
    }
}