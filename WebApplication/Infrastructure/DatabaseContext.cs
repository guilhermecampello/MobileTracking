using Microsoft.EntityFrameworkCore;
using MobileTracking.Core.Models;

namespace WebApplication.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Locale> Locales { get; set; } = null!;

        public DbSet<Zone> Zones { get; set; } = null!;

        public DbSet<Position> Positions { get; set; } = null!;

        public DbSet<PositionSignalData> PositionsData { get; set; } = null!;

        public DbSet<Calibration> Calibrations { get; set; } = null!;

        public DbSet<UserLocalization> UserLocalizations { get; set; } = null!;

        public DbSet<LocalizationMeasurement> LocalizationMeasurements { get; set; } = null!;
    }
}
