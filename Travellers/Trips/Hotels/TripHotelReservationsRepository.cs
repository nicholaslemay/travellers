using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travellers.Support.Db;

namespace Travellers.Trips.Hotels;

public interface ITripHotelReservationsRepository
{
    Task<IReadOnlyList<string>> GetHubReservationIdsAsync(Guid tripId, CancellationToken ct);
}

public class TripHotelReservationsRepository(DatabaseExecutor database) : ITripHotelReservationsRepository
{
    public Task<IReadOnlyList<string>> GetHubReservationIdsAsync(Guid tripId, CancellationToken ct) =>
        database.ExecuteAsync(async (context, token) =>
        {
            var ids = await context.Set<TripHotelReservationRow>()
                .Where(r => r.TripId == tripId)
                .Select(r => r.HubReservationId)
                .ToListAsync(token)
                .ConfigureAwait(false);

            return (IReadOnlyList<string>)ids;
        }, ct);
}

public class TripHotelReservationRow
{
    public Guid TripHotelReservationId { get; set; }
    public Guid TripId { get; set; }
    public string HubReservationId { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
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
