namespace SocialLibrary.API.Models
{
    // TMDb (Film) Cevap Modeli
    public class TmdbResponse
    {
        public List<TmdbMovie> results { get; set; }
    }

    public class TmdbMovie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string overview { get; set; } // Özet
        public string poster_path { get; set; }
        public string release_date { get; set; } // Yayın Yılı
        public double vote_average { get; set; } // Puan
        public int vote_count { get; set; }
    }

    // Google Books (Kitap) Cevap Modeli
    public class GoogleBooksResponse
    {
        public List<GoogleBookItem> items { get; set; }
    }

    public class GoogleBookItem
    {
        public string id { get; set; }
        public GoogleBookVolumeInfo volumeInfo { get; set; }
    }

    public class GoogleBookVolumeInfo
    {
        public string title { get; set; }
        public List<string> authors { get; set; } // Yazarlar
        public string description { get; set; } // Özet
        public string publishedDate { get; set; } // Yayın Tarihi
        public int pageCount { get; set; } // Sayfa Sayısı
        public GoogleBookImageLinks imageLinks { get; set; }
        public double? averageRating { get; set; } // Puan
    }

    public class GoogleBookImageLinks
    {
        public string thumbnail { get; set; }
    }
}
