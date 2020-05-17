using Microsoft.EntityFrameworkCore.Migrations;

namespace Violetum.Infrastructure.Migrations
{
    public partial class updateCategoryAndComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "ParentId",
                "Comments",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Categories",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                "Image",
                "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                "IX_Categories_Name",
                "Categories",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                "IX_Categories_Name",
                "Categories");

            migrationBuilder.DropColumn(
                "ParentId",
                "Comments");

            migrationBuilder.DropColumn(
                "Image",
                "Categories");

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Categories",
                "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}