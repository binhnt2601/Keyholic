using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanPhimCanhCach.Migrations
{
    public partial class modifyProVariant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "ProductVariant",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "ProductVariant");
        }
    }
}
