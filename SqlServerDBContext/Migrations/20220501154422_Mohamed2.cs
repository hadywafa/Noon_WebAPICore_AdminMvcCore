using Microsoft.EntityFrameworkCore.Migrations;

namespace SqlServerDBContext.Migrations
{
    public partial class Mohamed2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatusDescription",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryStatusDescription",
                table: "Orders");
        }
    }
}
