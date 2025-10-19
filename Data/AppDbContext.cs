using Microsoft.EntityFrameworkCore;
using YouTubeCompanion.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace YouTubeCompanion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<EventLog> EventLogs => Set<EventLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b => {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.GoogleUserId).IsUnique();
            });

            modelBuilder.Entity<UserToken>(b => {
                b.HasKey(t => t.Id);
                b.HasOne<User>().WithOne(u => u.Token!).HasForeignKey<UserToken>(t => t.UserId);
            });

            modelBuilder.Entity<Note>(b => {
                b.HasKey(n => n.Id);
                b.Property(n => n.Tags).HasColumnType("text[]");
            });

            modelBuilder.Entity<EventLog>(b => {
                b.HasKey(e => e.Id);
            });
        }
    }
}

