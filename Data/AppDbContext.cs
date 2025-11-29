using Microsoft.EntityFrameworkCore;
using projectAsp.Models;

namespace projectAsp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            // Use snake_case naming for PostgreSQL
            if (optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(x => x.UseNodaTime(false));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity for PostgreSQL
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                
                entity.HasKey(e => e.Id).HasName("pk_users");
                
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("uuid");
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("email");
                
                entity.Property(e => e.DisplayName)
                    .HasMaxLength(256)
                    .HasColumnName("display_name");
                
                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasColumnName("password_hash");
                
                entity.Property(e => e.PasswordResetToken)
                    .HasMaxLength(512)
                    .HasColumnName("password_reset_token");
                
                entity.Property(e => e.PasswordResetTokenExpires)
                    .HasColumnName("password_reset_token_expires");

                // Create a unique index on Email
                entity.HasIndex(e => e.Email).IsUnique().HasName("ix_users_email");
            });
        }
    }
}
