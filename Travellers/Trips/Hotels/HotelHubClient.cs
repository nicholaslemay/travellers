using System.Net.Http.Json;
using System.Text.Json;

namespace Travellers.Trips.Hotels;

public class HotelHubClient(HttpClient httpClient) : IHotelHubClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<HotelReservation> GetReservationAsync(string reservationId, CancellationToken ct)
    {
        var dto = await httpClient
            .GetFromJsonAsync<HotelHubReservationDto>($"v1/reservations/{reservationId}", JsonOptions, ct)
            .ConfigureAwait(false);

        return new HotelReservation(
            HotelName: dto!.Hotel.Name,
            CheckIn: dto.CheckInDate,
            CheckOut: dto.CheckOutDate
        );
    }

    private record HotelHubReservationDto(string ReservationId, HotelDto Hotel, DateOnly CheckInDate, DateOnly CheckOutDate);
    private record HotelDto(string Name);
}
