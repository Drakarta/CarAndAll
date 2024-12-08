using Microsoft.EntityFrameworkCore;
using Backend.Entities;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Voertuig> Voertuigen { get; set; }
        public DbSet<VerhuurAanvraag> VerhuurAanvragen { get; set; }
        public DbSet<VoertuigCategorie> VoertuigCategorie { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Voertuig>().ToTable("Voertuig");
            modelBuilder.Entity<Voertuig>().HasKey(v => v.VoertuigID);
            modelBuilder.Entity<VoertuigCategorie>().HasKey(v => v.Categorie);
            modelBuilder.Entity<VerhuurAanvraag>().HasKey(v => v.AanvraagID);

            modelBuilder.Entity<Voertuig>()
                .HasOne<VoertuigCategorie>(v => v.VoertuigCategorie)
                .WithMany(c => c.Voertuigen)
                .HasForeignKey(v => v.Categorie)
                .IsRequired();

            modelBuilder.Entity<VerhuurAanvraag>()
                .HasOne(v => v.Voertuig)
                .WithMany(v => v.VerhuurAanvragen)
                .HasForeignKey(v => v.VoertuigID)
                .IsRequired();
        }
    }
}