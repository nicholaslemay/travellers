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

        DbContext.Set<TripRow>().Add(new TripRow { TripId = tripId, CreatedAt = now, UpdatedAt = now });
        DbContext.Set<TripHotelReservationRow>().Add(new TripHotelReservationRow
        {
            TripHotelReservationId = Guid.NewGuid(),
            TripId = tripId,
            HubReservationId = "HB-2026-001234",
            CreatedAt = now,
            UpdatedAt = now
        });
        await DbContext.SaveChangesAsync();

        var response = await CreateHttpClient().GetAsync($"/trips/{tripId}/reservations/hotels");
        var body = await response.Content.ReadFromJsonAsync<GetHotelReservationsResponse>(JsonOptions);

        using var _ = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body!.TripId.Should().Be(tripId);
        body.Hotels.Should().HaveCount(1);
        body.Hotels[0].HotelName.Should().Be("Le Méridien Paris");
        body.Hotels[0].CheckIn.Should().Be(new DateOnly(2026, 8, 15));
        body.Hotels[0].CheckOut.Should().Be(new DateOnly(2026, 8, 18));
    }
}
