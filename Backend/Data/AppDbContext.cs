using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Crop> Crops { get; set; }
        public DbSet<CropListing> CropListings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Address :- 1
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithOne(u => u.Address)
                .HasForeignKey<Address>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //BankAccount :- 1 
            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.User)
                .WithOne(u => u.BankAccount)
                .HasForeignKey<BankAccount>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //CropListing :- 2
            modelBuilder.Entity<CropListing>()
                .HasOne(cl => cl.Crop)
                .WithMany()
                .HasForeignKey(cl => cl.CropId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CropListing>()
                .HasOne(cl => cl.Farmer)
                .WithMany()
                .HasForeignKey(cl => cl.FarmerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification:- 1
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            //Review :- 3
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Dealer)
                .WithMany()
                .HasForeignKey(r => r.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Farmer)
                .WithMany()
                .HasForeignKey(r => r.FarmerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Transaction)
                .WithMany()
                .HasForeignKey(r => r.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            //Subscription :- 2
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Dealer)
                .WithMany()
                .HasForeignKey(s => s.DealerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Crop)
                .WithMany()
                .HasForeignKey(s => s.CropId)
                .OnDelete(DeleteBehavior.Cascade);

            //Transaction :- 2
            modelBuilder.Entity<Transaction>()
                .HasOne(r => r.Dealer)
                .WithMany()
                .HasForeignKey(r => r.DealerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(r => r.Listing)
                .WithMany()
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            //User :- 2
            modelBuilder.Entity<User>()
                .HasOne(u => u.BankAccount)
                .WithOne(b => b.User)
                .HasForeignKey<User>(a => a.BankAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithOne(b => b.User)
                .HasForeignKey<User>(a => a.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }

    }
}
