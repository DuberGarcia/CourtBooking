using Domain.Courts;
using Domain.Reservations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class CourtBookingDbContext : DbContext
{
    public CourtBookingDbContext(DbContextOptions<CourtBookingDbContext> options) : base(options) { }

    public DbSet<Court> Courts => Set<Court>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourtBookingDbContext).Assembly);
    }
}