using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Models;

namespace SocialLibrary.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Icerik> Icerikler { get; set; } // Ana Set
        public DbSet<Film> Filmler { get; set; }     // Alt Set
        public DbSet<Kitap> Kitaplar { get; set; }   // Alt Set
        public DbSet<Aktivite> Aktiviteler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TPT Stratejisi: Icerikler tablosu ana tablo olsun, diğerleri ondan türesin.
            modelBuilder.Entity<Icerik>().UseTptMappingStrategy();
        }
    }
}