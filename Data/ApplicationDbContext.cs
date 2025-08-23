using Microsoft.EntityFrameworkCore;
using FootballBetting.Models;

namespace FootballBetting.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<FactTable> FactTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.User)
                .WithMany(u => u.Predictions)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.Match)
                .WithMany(m => m.Predictions)
                .HasForeignKey(p => p.MatchId);

            modelBuilder.Entity<FactTable>()
                .HasOne(f => f.User)
                .WithOne()
                .HasForeignKey<FactTable>(f => f.UserId);

            // Add unique constraint for user-match predictions
            modelBuilder.Entity<Prediction>()
                .HasIndex(p => new { p.UserId, p.MatchId })
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Fixed seed date (no DateTime.Now)
            var seedDate = new DateTime(2025, 01, 01);

            // Seed Teams with fixed CreatedAt
            var teams = new List<Team>
            {
                new Team { Id = 1, Name = "Manchester United", Logo = "mu.png", CreatedAt = seedDate },
                new Team { Id = 2, Name = "Liverpool", Logo = "liv.png", CreatedAt = seedDate },
                new Team { Id = 3, Name = "Arsenal", Logo = "ars.png", CreatedAt = seedDate },
                new Team { Id = 4, Name = "Chelsea", Logo = "che.png", CreatedAt = seedDate },
                new Team { Id = 5, Name = "Manchester City", Logo = "mc.png", CreatedAt = seedDate },
                new Team { Id = 6, Name = "Tottenham", Logo = "tot.png", CreatedAt = seedDate },
                new Team { Id = 7, Name = "Newcastle", Logo = "new.png", CreatedAt = seedDate },
                new Team { Id = 8, Name = "Brighton", Logo = "bri.png", CreatedAt = seedDate },
                new Team { Id = 9, Name = "West Ham", Logo = "wh.png", CreatedAt = seedDate },
                new Team { Id = 10, Name = "Aston Villa", Logo = "av.png", CreatedAt = seedDate },
                new Team { Id = 11, Name = "Crystal Palace", Logo = "cp.png", CreatedAt = seedDate },
                new Team { Id = 12, Name = "Everton", Logo = "eve.png", CreatedAt = seedDate },
                new Team { Id = 13, Name = "Fulham", Logo = "ful.png", CreatedAt = seedDate },
                new Team { Id = 14, Name = "Brentford", Logo = "bre.png", CreatedAt = seedDate },
                new Team { Id = 15, Name = "Wolves", Logo = "wol.png", CreatedAt = seedDate },
                new Team { Id = 16, Name = "Nottingham Forest", Logo = "nf.png", CreatedAt = seedDate },
                new Team { Id = 17, Name = "Bournemouth", Logo = "bou.png", CreatedAt = seedDate },
                new Team { Id = 18, Name = "Sheffield United", Logo = "su.png", CreatedAt = seedDate },
                new Team { Id = 19, Name = "Burnley", Logo = "bur.png", CreatedAt = seedDate },
                new Team { Id = 20, Name = "Luton Town", Logo = "lut.png", CreatedAt = seedDate }
            };

            modelBuilder.Entity<Team>().HasData(teams);

            // Matches with fixed dates
            var matches = new List<Match>
            {
                new Match { Id = 1, HomeTeamId = 1, AwayTeamId = 2, MatchDate = new DateTime(2025, 08, 10) },
                new Match { Id = 2, HomeTeamId = 3, AwayTeamId = 4, MatchDate = new DateTime(2025, 08, 11) },
                new Match { Id = 3, HomeTeamId = 5, AwayTeamId = 6, MatchDate = new DateTime(2025, 08, 12) },
                new Match { Id = 4, HomeTeamId = 7, AwayTeamId = 8, MatchDate = new DateTime(2025, 08, 13) },
                new Match { Id = 5, HomeTeamId = 9, AwayTeamId = 10, MatchDate = new DateTime(2025, 08, 14) },
                new Match { Id = 6, HomeTeamId = 11, AwayTeamId = 12, MatchDate = new DateTime(2025, 08, 15) },
                new Match { Id = 7, HomeTeamId = 13, AwayTeamId = 14, MatchDate = new DateTime(2025, 08, 16) },
                new Match { Id = 8, HomeTeamId = 15, AwayTeamId = 16, MatchDate = new DateTime(2025, 08, 17) },
                new Match { Id = 9, HomeTeamId = 17, AwayTeamId = 18, MatchDate = new DateTime(2025, 08, 18) },
                new Match { Id = 10, HomeTeamId = 19, AwayTeamId = 20, MatchDate = new DateTime(2025, 08, 19) }
            };

            modelBuilder.Entity<Match>().HasData(matches);
        }
    }
}
