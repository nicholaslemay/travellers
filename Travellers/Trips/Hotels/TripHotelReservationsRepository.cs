using Microsoft.EntityFrameworkCore;
using Travellers.Support.Db;

namespace Travellers.Trips.Hotels;

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
