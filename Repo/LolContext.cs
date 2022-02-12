using Core;
using Core.DMs;
using Microsoft.EntityFrameworkCore;

namespace Repo
{
    public class LolContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<MatchData> MatchDatas { get; set; }
        public DbSet<MatchDataComment> MotionActivities { get; set; }
        public DbSet<GlobalStatistics> GlobalStatistics { get; set; }

        public LolContext(DbContextOptions<LolContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchData>(entity =>
            {
                entity.ToTable("MatchData", "dbo");
                entity.HasKey(o => o.ID);
                entity.Property(o => o.ID).UseIdentityColumn();
            });

            modelBuilder.Entity<MatchDataComment>(entity =>
            {
                entity.ToTable("MatchDataComment", "dbo");
                entity.HasKey(o => o.ID);

                entity.HasOne(m => m.MatchData)
                .WithMany(d => d.MatchDataComments)
                .HasForeignKey(f => f.MatchDataID)
                .HasConstraintName("FK_MatchData_Comment_MatchData");
            });

            modelBuilder.Entity<GlobalStatistics>(entity =>
            {
                entity.ToTable("GlobalStatistics", "dbo");
                entity.HasKey(o => o.ID);
                entity.Property(o => o.ID).UseIdentityColumn();
            });
        }
    }
}