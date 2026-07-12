using FluentMigrator;

namespace Travellers.Support.Db.Migrations;

[Migration(20260712100000L)]
public class AddEmailToUsers : Migration
{
    public override void Up()
    {
        Alter.Table("users")
            .AddColumn("email").AsString(255).NotNullable()
            .AddColumn("created_at").AsDateTimeOffset().NotNullable()
            .AddColumn("updated_at").AsDateTimeOffset().NotNullable();

        Create.UniqueConstraint("uq_users_email").OnTable("users").Column("email");
    }

    public override void Down()
    {
        Delete.UniqueConstraint("uq_users_email").FromTable("users");
        Delete.Column("email").Column("created_at").Column("updated_at").FromTable("users");
    }
}
