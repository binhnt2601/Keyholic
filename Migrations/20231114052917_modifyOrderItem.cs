using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanPhimCanhCach.Migrations
{
    public partial class modifyOrderItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VariantName",
                table: "OrderItem",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariantName",
                table: "OrderItem");
        }
    }
}
