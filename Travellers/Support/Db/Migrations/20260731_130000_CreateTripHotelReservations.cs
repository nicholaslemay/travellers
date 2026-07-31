using FluentMigrator;

namespace Travellers.Support.Db.Migrations;

[Migration(20260731130000L)]
public class CreateTripHotelReservations : Migration
{
    public override void Up()
    {
        Create.Table("trip_hotel_reservations")
            .WithColumn("trip_hotel_reservation_id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("trip_id").AsGuid().NotNullable().ForeignKey("trips", "trip_id")
            .WithColumn("hub_reservation_id").AsString(255).NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("trip_hotel_reservations");
    }
}
