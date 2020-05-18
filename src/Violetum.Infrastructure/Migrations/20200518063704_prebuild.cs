using Microsoft.EntityFrameworkCore.Migrations;

namespace Violetum.Infrastructure.Migrations
{
    public partial class prebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Discriminator",
                "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Discriminator",
                "AspNetUsers",
                "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}