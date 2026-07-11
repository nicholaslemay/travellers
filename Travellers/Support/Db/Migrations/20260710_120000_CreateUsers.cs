using FluentMigrator;

namespace Travellers.Support.Db.Migrations;

[Migration(20260710120000L)]
public class CreateUsers : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("user_id").AsInt32().NotNullable().PrimaryKey().Identity();
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}
