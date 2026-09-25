using CourtBooking.Domain.Courts;
using CourtBooking.Domain.Reservations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourtBooking.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.CourtId).IsRequired();

        builder.Property(r => r.UserId).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        
        builder.ComplexProperty(r => r.Period, period =>
        {
            period.Property(p => p.Start).HasColumnName("start_at").IsRequired();
            period.Property(p => p.End).HasColumnName("end_at").IsRequired();
        });

        builder.HasOne<Court>()
            .WithMany()
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}