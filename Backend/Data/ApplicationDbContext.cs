using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Abonnement> Abonnement { get; set; }
        public DbSet<AbonnementAanvraag> AbonnementAanvragen { get; set; } // Ensure DbSet is properly added
        public DbSet<Bedrijf> Bedrijf { get; set; }
        public DbSet<BedrijfAccounts> BedrijfAccounts { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Voertuig> Voertuigen { get; set; }
        public DbSet<VerhuurAanvraag> VerhuurAanvragen { get; set; }
        public DbSet<VoertuigCategorie> VoertuigCategorie { get; set; }
        public DbSet<Schade> Schades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration for BedrijfAccounts
            modelBuilder.Entity<BedrijfAccounts>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            modelBuilder.Entity<BedrijfAccounts>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.BedrijfAccounts)
                .HasForeignKey(ab => ab.account_id);

            modelBuilder.Entity<BedrijfAccounts>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.BedrijfAccounts)
                .HasForeignKey(ab => ab.bedrijf_id);

            modelBuilder.Entity<Bedrijf>()
                .HasOne(b => b.abonnement)
                .WithMany()
                .HasForeignKey(b => b.AbonnementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration for Voertuig and VoertuigCategorie
            modelBuilder.Entity<Voertuig>().ToTable("Voertuig");
            modelBuilder.Entity<Voertuig>().HasKey(v => v.VoertuigID);
            modelBuilder.Entity<VoertuigCategorie>().HasKey(v => v.Categorie);
            modelBuilder.Entity<Voertuig>()
                .HasOne<VoertuigCategorie>(v => v.VoertuigCategorie)
                .WithMany(c => c.Voertuigen)
                .HasForeignKey(v => v.Categorie)
                .IsRequired();

            modelBuilder.Entity<VerhuurAanvraag>().HasKey(v => v.AanvraagID);
            modelBuilder.Entity<VerhuurAanvraag>().ToTable("VerhuurAanvraag");
            modelBuilder.Entity<VerhuurAanvraag>()
                .HasOne(v => v.Voertuig)
                .WithMany(v => v.VerhuurAanvragen)
                .HasForeignKey(v => v.VoertuigID)
                .IsRequired();

            // Configuration for Abonnement
            modelBuilder.Entity<Abonnement>().ToTable("Abonnement"); // Optional explicit table name
            modelBuilder.Entity<Abonnement>().HasKey(a => a.Id);

            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Naam)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Prijs_multiplier)
                .IsRequired();

            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Beschrijving)
                .HasMaxLength(500);

            modelBuilder.Entity<Abonnement>()
                .Property(a => a.Max_medewerkers)
                .IsRequired();

            // Configuration for AbonnementAanvraag
            modelBuilder.Entity<AbonnementAanvraag>()
                .ToTable("AbonnementAanvraag"); // Explicit table name
            modelBuilder.Entity<AbonnementAanvraag>()
                .HasKey(aa => aa.Id);

            modelBuilder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Naam)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Beschrijving)
                .HasMaxLength(500);

            modelBuilder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.PrijsMultiplier)
                .IsRequired();

            modelBuilder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.MaxMedewerkers)
                .IsRequired();

            modelBuilder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Status)
                .HasDefaultValue("In behandeling");

            modelBuilder.Entity<AbonnementAanvraag>()
            .HasOne(aa => aa.Bedrijf)
            .WithMany(b => b.AbonnementAanvragen)
            .HasForeignKey(aa => aa.BedrijfId)
            .IsRequired();
        }
    }
}
