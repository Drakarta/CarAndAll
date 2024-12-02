using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarAndAll
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bedrijf> Bedrijven { get; set; }
        public DbSet<AccountBedrijf> AccountBedrijven { get; set; }
        public DbSet<Email> Emails { get; set; } // Fix this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the many-to-many relationship between Account and Bedrijf
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