using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend.Entities;

namespace Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bedrijf> Bedrijf { get; set; }
        public DbSet<AccountBedrijf> AccountBedrijven { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Voertuig> Voertuigen { get; set; }
        public DbSet<VerhuurAanvraag> VerhuurAanvragen { get; set; }
        public DbSet<VoertuigCategorie> VoertuigCategorie { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AccountBedrijf>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            modelBuilder.Entity<AccountBedrijf>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.AccountBedrijven)
                .HasForeignKey(ab => ab.account_id);

            modelBuilder.Entity<AccountBedrijf>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.AccountBedrijven)
                .HasForeignKey(ab => ab.bedrijf_id);

            modelBuilder.Entity<Voertuig>().ToTable("Voertuig");
            modelBuilder.Entity<Voertuig>().HasKey(v => v.VoertuigID);
            modelBuilder.Entity<VoertuigCategorie>().HasKey(v => v.Categorie);
            modelBuilder.Entity<VerhuurAanvraag>().HasKey(v => v.AanvraagID);
            modelBuilder.Entity<VerhuurAanvraag>().ToTable("VerhuurAanvraag");

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