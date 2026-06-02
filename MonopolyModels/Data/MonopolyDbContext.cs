using Microsoft.EntityFrameworkCore;
using MonopolyModels.Entities;

namespace MonopolyModels.Data;

public class MonopolyDbContext : DbContext
{
    private readonly string? _databasePath;

    public MonopolyDbContext()
    {
    }

    public MonopolyDbContext(string databasePath)
    {
        _databasePath = databasePath;
    }

    public MonopolyDbContext(DbContextOptions<MonopolyDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<MapCell> MapCells => Set<MapCell>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<EventCard> EventCards => Set<EventCard>();
    public DbSet<GameRecord> GameRecords => Set<GameRecord>();
    public DbSet<PlayerGameRecord> PlayerGameRecords => Set<PlayerGameRecord>();
    public DbSet<ActionRecord> ActionRecords => Set<ActionRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var path = _databasePath ?? DbPaths.GetDefaultDatabasePath();
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        optionsBuilder.UseSqlite($"Data Source={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(x => x.UserName)
            .IsUnique();

        modelBuilder.Entity<MapCell>()
            .HasIndex(x => x.CellIndex)
            .IsUnique();

        modelBuilder.Entity<Property>()
            .HasIndex(x => x.MapCellId)
            .IsUnique();

        modelBuilder.Entity<Property>()
            .HasOne(x => x.MapCell)
            .WithMany()
            .HasForeignKey(x => x.MapCellId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GameRecord>()
            .HasMany(x => x.PlayerGameRecords)
            .WithOne(x => x.GameRecord)
            .HasForeignKey(x => x.GameRecordId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GameRecord>()
            .HasMany(x => x.ActionRecords)
            .WithOne(x => x.GameRecord)
            .HasForeignKey(x => x.GameRecordId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
