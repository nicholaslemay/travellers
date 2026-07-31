using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Travellers.Trips.Hotels;

public class TripHotelReservationRowConfiguration : IEntityTypeConfiguration<TripHotelReservationRow>
{
    public void Configure(EntityTypeBuilder<TripHotelReservationRow> builder)
    {
        builder.ToTable("trip_hotel_reservations");
        builder.HasKey(r => r.TripHotelReservationId);
        builder.Property(r => r.TripHotelReservationId).HasColumnName("trip_hotel_reservation_id").ValueGeneratedNever();
        builder.Property(r => r.TripId).HasColumnName("trip_id");
        builder.Property(r => r.HubReservationId).HasColumnName("hub_reservation_id");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.HasOne<TripRow>().WithMany().HasForeignKey(r => r.TripId);
    }
}
