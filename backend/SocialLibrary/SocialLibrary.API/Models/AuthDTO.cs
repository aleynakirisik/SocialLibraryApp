namespace SocialLibrary.API.Models
{
    public class KayitDto
    {
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
    }

    public class GirisDto
    {
        public string Email { get; set; }
        public string Sifre { get; set; }
    }

    public class SifreUnuttumDto
    {
        public string Email { get; set; }
    }

    public class SifreSifirlaDto
    {
        public string Email { get; set; }
        public string YeniSifre { get; set; }
    }
}