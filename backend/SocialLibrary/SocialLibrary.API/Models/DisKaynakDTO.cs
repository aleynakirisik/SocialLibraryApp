namespace SocialLibrary.API.Models
{
    public class TmdbMovieDetail
    {
        public int id { get; set; }
        public string title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public double vote_average { get; set; }
        public int runtime { get; set; } 
        public List<TmdbGenre> genres { get; set; }
        public TmdbCredits credits { get; set; } 
    }

    public class TmdbGenre { public string name { get; set; } }

    public class TmdbCredits
    {
        public List<TmdbCrew> crew { get; set; }
        public List<TmdbCast> cast { get; set; }
    }

    public class TmdbCrew
    {
        public string name { get; set; }
        public string job { get; set; } 
    }

    public class TmdbCast { public string name { get; set; } }

    public class GoogleBooksResponse { public List<GoogleBookItem> items { get; set; } }
    public class GoogleBookItem { public string id { get; set; } public GoogleBookVolumeInfo volumeInfo { get; set; } }
    public class GoogleBookVolumeInfo
    {
        public string title { get; set; }
        public List<string> authors { get; set; }
        public string description { get; set; }
        public string publishedDate { get; set; }
        public int pageCount { get; set; }
        public List<string> categories { get; set; } 
        public GoogleBookImageLinks imageLinks { get; set; }
        public double? averageRating { get; set; }
    }
    public class GoogleBookImageLinks { public string thumbnail { get; set; } }

    public class TmdbResponse { public List<TmdbMovie> results { get; set; } }

    public class TmdbMovie
    {
        public int id { get; set; }
        public string title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public double vote_average { get; set; }
    }
}