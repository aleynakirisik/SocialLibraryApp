using System.ComponentModel.DataAnnotations.Schema;

namespace SocialLibrary.API.Models
{
    // 1. ANA SINIF (Ortak Özellikler) -> Icerikler Tablosu Olacak
    public abstract class Icerik
    {
        public int Id { get; set; }
        public string DisKaynakId { get; set; } = string.Empty; // API ID'si
        public string Baslik { get; set; } = string.Empty;
        public string GorselUrl { get; set; } = string.Empty;
        public string? Aciklama { get; set; } // Özet
        public string? YayinYili { get; set; }
        public double? Puan { get; set; } // 10 üzerinden

        // Bu kayıt ne türde? (Frontend'de ayrım yapmak için)
        public string IcerikTuru { get; set; } = string.Empty;
    }

    // 2. FİLM SINIFI (Icerik'ten miras alır) -> Filmler Tablosu Olacak
    [Table("Filmler")]
    public class Film : Icerik
    {
        // Sadece filme özel alanlar
        public string? Yonetmen { get; set; }
        public string? Oyuncular { get; set; } // Virgülle ayrılmış isimler
        public string? Turler { get; set; } // Aksiyon, Dram vs.

        public Film() { IcerikTuru = "Film"; } // Otomatik etiketle
    }

    // 3. KİTAP SINIFI (Icerik'ten miras alır) -> Kitaplar Tablosu Olacak
    [Table("Kitaplar")]
    public class Kitap : Icerik
    {
        // Sadece kitaba özel alanlar
        public string? Yazarlar { get; set; }
        public int? SayfaSayisi { get; set; }
        public string? Yayinevi { get; set; }

        public Kitap() { IcerikTuru = "Kitap"; } // Otomatik etiketle
    }

    // --- DİĞER TABLOLARIN AYNI KALABİLİR ---
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string ProfilResmiUrl { get; set; } = "https://via.placeholder.com/50";
        public string? Biyografi { get; set; }
    }

    // Aktivite tablosunda hiçbir şeyi değiştirmene gerek yok!
    // Çünkü "Item" (Icerik) tablosuna bağlı, o da zaten ana tablo.
    public class Aktivite
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public Kullanici? Kullanici { get; set; }

        public int IcerikId { get; set; }
        public Icerik? Icerik { get; set; } // Hem Kitap hem Film buraya bağlanabilir!

        public string EylemTuru { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public double? Puan { get; set; }
        public string? Yorum { get; set; }
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.UtcNow;
    }
}