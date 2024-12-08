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
        }
    }
}

