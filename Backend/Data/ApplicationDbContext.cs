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

            modelBuilder.Entity<VerhuurAanvraag>()
                .HasOne(v => v.Account)
                .WithMany(v => v.VerhuurAanvragen)
                .HasForeignKey(v => v.GebruikerID)
                .IsRequired();
        }
    }
}