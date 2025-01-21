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
        public DbSet<Bedrijf> Bedrijf { get; set; }
        public DbSet<BedrijfAccounts> BedrijfAccounts { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Voertuig> Voertuigen { get; set; }
        public DbSet<VerhuurAanvraag> VerhuurAanvragen { get; set; }
        // public DbSet<VoertuigCategorie> VoertuigCategorie { get; set; }
        public DbSet<Schade> Schades { get; set; }
        public DbSet<BedrijfWagenparkbeheerders> BedrijfWagenparkbeheerders { get; set; }

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

            modelBuilder.Entity<BedrijfWagenparkbeheerders>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            modelBuilder.Entity<BedrijfWagenparkbeheerders>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.BedrijfWagenparkbeheerders)
                .HasForeignKey(ab => ab.account_id);
;

            modelBuilder.Entity<BedrijfWagenparkbeheerders>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.BedrijfWagenparkbeheerders)
                .HasForeignKey(ab => ab.bedrijf_id);


            modelBuilder.Entity<Voertuig>().ToTable("Voertuig");
            modelBuilder.Entity<Voertuig>().HasKey(v => v.VoertuigID);
            modelBuilder.Entity<VerhuurAanvraag>().HasKey(v => v.AanvraagID);
            modelBuilder.Entity<VerhuurAanvraag>().ToTable("VerhuurAanvraag");

            modelBuilder.Entity<Voertuig>()
                .HasDiscriminator<string>("voertuig_categorie")
                .HasValue<Auto>("Auto")
                .HasValue<Caravan>("Caravan")
                .HasValue<Camper>("Camper");

            modelBuilder.Entity<Voertuig>()
                .Property(v => v.voertuig_categorie)
                .HasColumnName("voertuig_categorie")
                .HasMaxLength(8)
                .IsRequired();

            modelBuilder.Entity<VerhuurAanvraag>()
                .HasOne(v => v.Voertuig)
                .WithMany(v => v.VerhuurAanvragen)
                .HasForeignKey(v => v.VoertuigID)
                .IsRequired();

        }
    }
}