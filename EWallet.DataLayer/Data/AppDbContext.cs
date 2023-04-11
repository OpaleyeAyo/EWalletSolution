using EWallet.Entities.DbEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EWallet.DataLayer.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> accountDbOptions) : base(accountDbOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>()
            .Property(b => b.AccountBalance)
            .HasPrecision(30, 6);

            modelBuilder.Entity<Transaction>()
            .Property(b => b.Amount)
            .HasPrecision(30, 6);

            modelBuilder.Entity<SettlementAccount>()
            .Property(b => b.AccountBalance)
            .HasPrecision(30, 6);

            modelBuilder.Entity<Customer>()
                .HasOne<ProfilePhoto>(s => s.ProfilePhoto)
                .WithOne(y => y.Customer)
                .HasForeignKey<ProfilePhoto>(y => y.CustomerId);

            modelBuilder.Entity<Wallet>()
             .HasOne<Customer>(s => s.Customer)
             .WithMany(g => g.Wallets)
             .HasForeignKey(s => s.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Transaction>()
            // .HasOne<Wallet>(s => s.Wallet)
            // .WithMany(g => g.Transactions)
            // .HasForeignKey(s => s.WalletId)
            // .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Currency>()
                .HasOne<CurrencyLogo>(s => s.CurrencyLogo)
                .WithOne(y => y.Currency)
                .HasForeignKey<CurrencyLogo>(y => y.CurrencyId);
        }

        public DbSet<SettlementAccount> SettlementAccounts { get; set; }

        public DbSet<Currency> Currencies { get; set; }
        
        public DbSet<Customer> Customers { get; set; }
        
        public DbSet<ProfilePhoto> ProfilePhotos { get; set; }

        public DbSet<CurrencyLogo> CurrencyLogos { get; set; }
        
        public DbSet<Wallet> Wallets { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
