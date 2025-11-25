using System.ComponentModel.DataAnnotations;

namespace SocialLibrary.API.Models
{
    // Eski "User" -> Yeni "Kullanici"
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string ProfilResmiUrl { get; set; } = "https://via.placeholder.com/50";
    }

    // Eski "Item" -> Yeni "Icerik" (Kitap veya Film verisi)
    public class Icerik
    {
        public int Id { get; set; }
        public string DisKaynakId { get; set; } = string.Empty; // Google Books veya TMDb ID'si
        public string Tur { get; set; } = string.Empty; // "Film" veya "Kitap"
        public string Baslik { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty; // Kapak resmi
    }

    // Eski "Activity" -> Yeni "Aktivite" (Akışta görünecek hareketler)
    public class Aktivite
    {
        public int Id { get; set; }

        // Hangi Kullanıcı Yaptı?
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        // Hangi İçerik (Film/Kitap) İçin?
        public int IcerikId { get; set; }
        public Icerik? Icerik { get; set; }

        public string EylemTuru { get; set; } = string.Empty; // "PUANLAMA", "YORUM"
        public string Aciklama { get; set; } = string.Empty; // Örn: "...bir filmi oyladı"

        public double? Puan { get; set; } // Puan verdiyse kaç verdi?
        public string? Yorum { get; set; } // Yorum yaptıysa ne yazdı?

        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    }
}