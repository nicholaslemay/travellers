extern alias HotelHubFake;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TravellersTest.Support;

public class HotelHubFakeServer : WebApplicationFactory<HotelHubFake::Travellers.HotelHubFake.HotelHubFakeMarker>
{
}
