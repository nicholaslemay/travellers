using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Travellers.Trips;
using Travellers.Trips.Hotels;
using Travellers.Trips.Hotels.GetHotelReservations;
using TravellersTest.Support;

namespace TravellersTest.Trips.Hotels;

[Collection("Database")]
public class GetHotelReservationsEndpointTests : DatabaseTest, IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HotelHubFakeServer _fakeServer = new();

    public GetHotelReservationsEndpointTests(DatabaseMigrationFixture fixture) : base(fixture)
    {
        OverrideConfiguration("HotelHub:BaseUrl", _fakeServer.BaseUrl);
    }

    public void Dispose() => _fakeServer.Dispose();

    [Fact]
    public async Task ShouldReturn200WithHotelReservations()
    {
        var tripId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        _fakeServer.AddReservation("HB-2026-001234", "Le Méridien Paris", new DateOnly(2026, 8, 15), new DateOnly(2026, 8, 18));
        _fakeServer.AddReservation("HB-2026-005678", "Hotel Le Bristol Paris", new DateOnly(2026, 8, 19), new DateOnly(2026, 8, 22));

        DbContext.Set<TripRow>().Add(new TripRow { TripId = tripId, CreatedAt = now, UpdatedAt = now });
        DbContext.Set<TripHotelReservationRow>().AddRange(
            new TripHotelReservationRow { TripHotelReservationId = Guid.NewGuid(), TripId = tripId, HubReservationId = "HB-2026-001234", CreatedAt = now, UpdatedAt = now },
            new TripHotelReservationRow { TripHotelReservationId = Guid.NewGuid(), TripId = tripId, HubReservationId = "HB-2026-005678", CreatedAt = now, UpdatedAt = now }
        );
        await DbContext.SaveChangesAsync();

        var response = await CreateHttpClient().GetAsync($"/trips/{tripId}/reservations/hotels");
        var body = await response.Content.ReadFromJsonAsync<GetHotelReservationsResponse>(JsonOptions);

        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.TripId.Should().Be(tripId);
        body.Hotels.Should().BeEquivalentTo(new[]
        {
            new { HotelName = "Le Méridien Paris",      CheckIn = new DateOnly(2026, 8, 15), CheckOut = new DateOnly(2026, 8, 18) },
            new { HotelName = "Hotel Le Bristol Paris", CheckIn = new DateOnly(2026, 8, 19), CheckOut = new DateOnly(2026, 8, 22) }
        });
    }
}
