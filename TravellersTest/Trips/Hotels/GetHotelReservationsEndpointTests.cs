using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using Travellers.Trips.Hotels;
using TravellersTest.Support;

namespace TravellersTest.Trips.Hotels;

[Collection("Database")]
public class GetHotelReservationsEndpointTests(DatabaseMigrationFixture fixture) : DatabaseTest(fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldReturn200WithHotelReservations()
    {
        var tripId = Guid.NewGuid();
        var client = CreateHttpClient();

        var response = await client.GetAsync($"/trips/{tripId}/reservations/hotels");
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
