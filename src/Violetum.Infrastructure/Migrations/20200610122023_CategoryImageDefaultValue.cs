using Microsoft.EntityFrameworkCore.Migrations;

namespace Violetum.Infrastructure.Migrations
{
    public partial class CategoryImageDefaultValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Image",
                "Categories",
                nullable: true,
                defaultValue: "Category/no-image.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "Image",
                "Categories",
                "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "Category/no-image.jpg");
        }
    }
}