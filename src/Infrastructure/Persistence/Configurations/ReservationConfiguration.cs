
using Domain.Courts;
using Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.CourtId).IsRequired();

        builder.Property(b => b.UserId).IsRequired();

        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.ComplexProperty(b => b.TimeRange, TimeRange =>
        {
            TimeRange.Property(p => p.Start).HasColumnName("start_time").IsRequired();
            TimeRange.Property(p => p.End).HasColumnName("end_time").IsRequired();
        });

        builder.HasOne<Court>()
            .WithMany()
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}