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
        public DbSet<Voertuig> Voertuigen { get; set; }
        public DbSet<VerhuurAanvraag> VerhuurAanvragen { get; set; }
        public DbSet<VerhuurAanvraagAccessoire> VerhuurAanvraagAccessoires { get; set; }
        public DbSet<Accessoire> Accessoires { get; set; }
        public DbSet<Schade> Schades { get; set; }
        public DbSet<BedrijfWagenparkbeheerders> BedrijfWagenparkbeheerders { get; set; }
        public DbSet<Text> Texts { get; set; }

        //Hier wordt de relatie tussen de tabellen vastgelegd

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BedrijfAccounts>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            builder.Entity<BedrijfAccounts>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.BedrijfAccounts)
                .HasForeignKey(ab => ab.account_id);

            builder.Entity<BedrijfAccounts>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.BedrijfAccounts)
                .HasForeignKey(ab => ab.bedrijf_id);

            builder.Entity<BedrijfWagenparkbeheerders>()
                .HasKey(ab => new { ab.account_id, ab.bedrijf_id });

            builder.Entity<BedrijfWagenparkbeheerders>()
                .HasOne(ab => ab.Account)
                .WithMany(a => a.BedrijfWagenparkbeheerders)
                .HasForeignKey(ab => ab.account_id);

            builder.Entity<BedrijfWagenparkbeheerders>()
                .HasOne(ab => ab.Bedrijf)
                .WithMany(b => b.BedrijfWagenparkbeheerders)
                .HasForeignKey(ab => ab.bedrijf_id);

            builder.Entity<Voertuig>().ToTable("Voertuig");
            builder.Entity<Voertuig>().HasKey(v => v.VoertuigID);
            builder.Entity<VerhuurAanvraag>().HasKey(v => v.AanvraagID);
            builder.Entity<Accessoire>().HasKey(v => v.AccessoireNaam);
            builder.Entity<VerhuurAanvraag>().ToTable("VerhuurAanvraag");

            builder.Entity<VerhuurAanvraagAccessoire>()
                .HasKey(vaa => new { vaa.AanvraagID, vaa.AccessoireNaam });

            builder.Entity<Voertuig>()
                .HasDiscriminator<string>("voertuig_categorie")
                .HasValue<Auto>("Auto")
                .HasValue<Caravan>("Caravan")
                .HasValue<Camper>("Camper");

            builder.Entity<Voertuig>()
                .Property(v => v.voertuig_categorie)
                .HasColumnName("voertuig_categorie")
                .HasMaxLength(8)
                .IsRequired();

            builder.Entity<VerhuurAanvraagAccessoire>()
                .HasOne(v => v.VerhuurAanvraag)
                .WithMany(v => v.VerhuurAanvraagAccessoires)
                .HasForeignKey(v => v.AanvraagID)
                .IsRequired();

            builder.Entity<VerhuurAanvraagAccessoire>()
                .HasOne(v => v.Accessoire)
                .WithMany(v => v.VerhuurAanvraagAccessoires)
                .HasForeignKey(v => v.AccessoireNaam)
                .IsRequired();

            builder.Entity<VerhuurAanvraag>()
                .HasOne(v => v.Voertuig)
                .WithMany(v => v.VerhuurAanvragen)
                .HasForeignKey(v => v.VoertuigID)
                .IsRequired();
            builder.Entity<AbonnementAanvraag>()
                           .ToTable("AbonnementAanvraag"); // Explicit table name
            builder.Entity<AbonnementAanvraag>()
                .HasKey(aa => aa.Id);

            builder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Naam)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Beschrijving)
                .HasMaxLength(500);

            builder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.PrijsMultiplier)
                .IsRequired();

            builder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.MaxMedewerkers)
                .IsRequired();

            builder.Entity<AbonnementAanvraag>()
                .Property(aa => aa.Status)
                .HasDefaultValue("In behandeling");

            builder.Entity<AbonnementAanvraag>()
                .HasOne(aa => aa.Bedrijf)
                .WithMany(b => b.AbonnementAanvragen)
                .HasForeignKey(aa => aa.BedrijfId)
                .IsRequired();

            builder.Entity<Abonnement>()
                .ToTable("Abonnement");

            builder.Entity<Abonnement>()
                .HasMany(a => a.Bedrijven)
                .WithOne(b => b.abonnement)
                .HasForeignKey(b => b.AbonnementId)
                .IsRequired(false);

            builder.Entity<Text>()
                .HasKey(t => t.Type);
        }
    }
}
