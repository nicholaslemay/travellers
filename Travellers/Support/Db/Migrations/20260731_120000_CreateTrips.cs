using FluentMigrator;

namespace Travellers.Support.Db.Migrations;

[Migration(20260731120000L)]
public class CreateTrips : Migration
{
    public override void Up()
    {
        Create.Table("trips")
            .WithColumn("trip_id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable()
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("trips");
    }
}
