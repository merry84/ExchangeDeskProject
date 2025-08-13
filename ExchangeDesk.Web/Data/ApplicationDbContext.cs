using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExchangeDesk.Web.Models;
using System.Collections.Generic;
namespace ExchangeDesk.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
      
        public DbSet<Office> Offices => Set<Office>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Ключ за Currency = Code
            builder.Entity<Currency>()
                   .HasKey(c => c.Code);

            // Seed данни
            builder.Entity<Office>().HasData(
             
                new Office { Id = 1, Name = "Централен офис", Location = "София", IsCentral = true },
                new Office { Id = 2, Name = "Офис 2", Location = "Пловдив", IsCentral = false }
                
               
            );

            builder.Entity<Currency>().HasData(
                new Currency { Code = "BGN", Name = "Bulgarian Lev", IsActive = true },
                new Currency { Code = "EUR", Name = "Euro", IsActive = true },
                new Currency { Code = "USD", Name = "US Dollar", IsActive = true },
                new Currency { Code = "GBP", Name = "British Pound", IsActive = true }
            );

            builder.Entity<ExchangeRate>()
                .HasOne(er => er.Office)
                .WithMany()
                .HasForeignKey(er => er.OfficeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ExchangeRate>()
                .HasOne(er => er.Currency)
                .WithMany()
                .HasForeignKey(er => er.CurrencyCode)
                .OnDelete(DeleteBehavior.Cascade);
            var now = DateTime.UtcNow;

            builder.Entity<ExchangeRate>().HasData(
                new ExchangeRate
                {
                    Id = 1,
                    OfficeId = 1,
                    CurrencyCode = "EUR",
                    RateToBGN = 1.95583m,
                    BuyRate = 1.9550m,
                    SellRate = 1.9570m,
                    AsOf = now
                },
                new ExchangeRate
                {
                    Id = 2,
                    OfficeId = 1,
                    CurrencyCode = "USD",
                    RateToBGN = 1.80m,
                    BuyRate = 1.7950m,
                    SellRate = 1.8050m,
                    AsOf = now
                }
            );


        }
    }
}

