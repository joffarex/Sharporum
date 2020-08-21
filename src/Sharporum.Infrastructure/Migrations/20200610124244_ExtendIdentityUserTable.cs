using Microsoft.EntityFrameworkCore.Migrations;

namespace Sharporum.Infrastructure.Migrations
{
    public partial class ExtendIdentityUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "BirthDate",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "FirstName",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Gender",
                "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Image",
                "AspNetUsers",
                nullable: true,
                defaultValue: "Category/no-image.jpg");

            migrationBuilder.AddColumn<string>(
                "LastName",
                "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "BirthDate",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "FirstName",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "Gender",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "Image",
                "AspNetUsers");

            migrationBuilder.DropColumn(
                "LastName",
                "AspNetUsers");
        }
    }
}