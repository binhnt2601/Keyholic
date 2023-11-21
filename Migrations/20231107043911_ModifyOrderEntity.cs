using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanPhimCanhCach.Migrations
{
    public partial class ModifyOrderEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Order",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Order");
        }
    }
}
