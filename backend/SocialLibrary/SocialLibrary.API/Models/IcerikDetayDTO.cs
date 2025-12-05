namespace SocialLibrary.API.Models
{
    public class IcerikDetayDto
    {
        public Icerik Icerik { get; set; }
        public double PlatformPuani { get; set; }
        public int ToplamOy { get; set; }
        public bool ListemdeMi { get; set; } 
        public string KullaniciPuani { get; set; } 
    }

    public class YorumGuncelleDto
    {
        public int YorumId { get; set; }
        public string YeniMetin { get; set; }
    }
}