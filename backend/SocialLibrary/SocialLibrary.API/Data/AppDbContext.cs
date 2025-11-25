using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablo İsimleri de Türkçe Oldu
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Icerik> Icerikler { get; set; }
        public DbSet<Aktivite> Aktiviteler { get; set; }
    }
}