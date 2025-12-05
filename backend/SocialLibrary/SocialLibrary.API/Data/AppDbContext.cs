using Microsoft.EntityFrameworkCore;
using SocialLibrary.API.Models;


namespace SocialLibrary.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Icerik> Icerikler { get; set; } 
        public DbSet<Film> Filmler { get; set; }     
        public DbSet<Kitap> Kitaplar { get; set; }  
        public DbSet<Aktivite> Aktiviteler { get; set; }
        public DbSet<Takip> Takipler { get; set; }
        public DbSet<OzelListe> OzelListeler { get; set; }
        public DbSet<OzelListeOgesi> OzelListeOgeleri { get; set; }
        public DbSet<Begeni> Begeniler { get; set; }
        public DbSet<AktiviteYorumu> AktiviteYorumlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Icerik>().UseTptMappingStrategy();
        }
    }
}