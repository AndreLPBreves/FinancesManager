using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<LedgerAccessLevel> LedgerAccessLevels => Set<LedgerAccessLevel>();
        public DbSet<Ledger> Ledgers => Set<Ledger>();
        public DbSet<LedgerAllowedUser> LedgerAllowedUsers => Set<LedgerAllowedUser>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<EmailConfirmation> EmailConfirmations => Set<EmailConfirmation>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.FirstName).HasMaxLength(User.firstNameMaxLength);
                entity.Property(u => u.LastName).HasMaxLength(User.lastNameMaxLength);
                entity.Property(u => u.Email).HasMaxLength(User.emailMaxLength);
                entity
                    .Property(u => u.PasswordSalt)
                    .HasMaxLength(PasswordHasherService.PasswordSaltLength);
                entity
                    .Property(u => u.PasswordHash)
                    .HasMaxLength(PasswordHasherService.PasswordHashLength);
            });

            modelBuilder.Entity<LedgerAccessLevel>(entity =>
            {
                entity.Property(e => e.Id);
                entity.HasIndex(e => e.Description).IsUnique();
                entity
                    .Property(e => e.Description)
                    .HasMaxLength(LedgerAccessLevel.descriptionMaxLengh);
            });

            modelBuilder.Entity<Ledger>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasOne(f => f.Owner).WithMany(o => o.Ledgers).HasForeignKey(f => f.OwnerId);
                entity.HasIndex(l => new { l.OwnerId, l.Name }).IsUnique();
            });

            modelBuilder.Entity<LedgerAllowedUser>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LedgerId });
                entity
                    .HasOne(e => e.User)
                    .WithMany(e => e.AccessibleLedgers)
                    .HasForeignKey(e => e.UserId);
                entity
                    .HasOne(e => e.Ledger)
                    .WithMany(e => e.AllowedUsers)
                    .HasForeignKey(e => e.LedgerId);
                entity
                    .HasOne(e => e.AccessLevel)
                    .WithMany(e => e.AllowedUsers)
                    .HasForeignKey(e => e.AccessLevelId);
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User).WithMany(e => e.Sessions).HasForeignKey(e => e.UserId);
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<EmailConfirmation>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity
                    .HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<EmailConfirmation>(e => e.UserId);
                entity.HasIndex(e => e.Key).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
