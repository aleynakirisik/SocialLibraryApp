using System.ComponentModel.DataAnnotations.Schema;

namespace SocialLibrary.API.Models
{
    public abstract class Icerik
    {
        public int Id { get; set; }
        public string DisKaynakId { get; set; } = string.Empty;
        public string Baslik { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public string? Aciklama { get; set; }
        public string? YayinYili { get; set; }
        public double? Puan { get; set; } 
        public string IcerikTuru { get; set; } = string.Empty;
    }

    [Table("Filmler")]
    public class Film : Icerik
    {
        public string? Yonetmen { get; set; }
        public string? Oyuncular { get; set; }
        public string? Turler { get; set; }
        public int? Sure { get; set; }
        public Film() { IcerikTuru = "Film"; }
    }

    [Table("Kitaplar")]
    public class Kitap : Icerik
    {
        public string? Yazarlar { get; set; }
        public int? SayfaSayisi { get; set; }
        public string? Yayinevi { get; set; }
        public string? Turler { get; set; }
        public Kitap() { IcerikTuru = "Kitap"; }
    }

    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string ProfilResmiUrl { get; set; } = "https://via.placeholder.com/50";
        public string? Biyografi { get; set; }
    }

    public class Takip
    {
        public int Id { get; set; }
        public int TakipEdenKullaniciId { get; set; }
        public int TakipEdilenKullaniciId { get; set; }
        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }

    public class Aktivite
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        public int IcerikId { get; set; }
        public Icerik? Icerik { get; set; }
        public string EylemTuru { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public double? Puan { get; set; }
        public string? Yorum { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    }

    public class Begeni
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int AktiviteId { get; set; }
    }

    public class AktiviteYorumu
    {
        public int Id { get; set; }
        public int AktiviteId { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }
        public string Yorum { get; set; } = string.Empty;
        public DateTime Tarih { get; set; } = DateTime.UtcNow;
    }

    public class OzelListe
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public int KullaniciId { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    }

    public class OzelListeOgesi
    {
        public int Id { get; set; }
        public int OzelListeId { get; set; }
        public int IcerikId { get; set; }
        public Icerik? Icerik { get; set; }
    }
}