using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sharporum.Infrastructure.Migrations
{
    public partial class updateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Categories_AspNetUsers_AuthorId",
                "Categories");

            migrationBuilder.DropForeignKey(
                "FK_Posts_Categories_CategoryId",
                "Posts");

            migrationBuilder.DropIndex(
                "IX_Posts_CategoryId",
                "Posts");

            migrationBuilder.DropIndex(
                "IX_Categories_AuthorId",
                "Categories");

            migrationBuilder.DropIndex(
                "IX_Categories_Name",
                "Categories");

            migrationBuilder.DropColumn(
                "CategoryId",
                "Posts");

            migrationBuilder.DropColumn(
                "AuthorId",
                "Categories");

            migrationBuilder.DropColumn(
                "Description",
                "Categories");

            migrationBuilder.DropColumn(
                "Image",
                "Categories");

            migrationBuilder.AddColumn<string>(
                "CommunityId",
                "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ContentType",
                "Posts",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Categories",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                "Image",
                "AspNetUsers",
                nullable: true,
                defaultValue: "Community/no-image.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "Category/no-image.jpg");

            migrationBuilder.CreateTable(
                "Communities",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>("ntext", nullable: true),
                    Image = table.Column<string>(nullable: true, defaultValue: "Community/no-image.jpg"),
                    AuthorId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        "FK_Communities_AspNetUsers_AuthorId",
                        x => x.AuthorId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "CommunityCategories",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CommunityId = table.Column<string>(nullable: true),
                    CategoryId = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityCategories", x => x.Id);
                    table.ForeignKey(
                        "FK_CommunityCategories_Categories_CategoryId",
                        x => x.CategoryId,
                        "Categories",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_CommunityCategories_Communities_CommunityId",
                        x => x.CommunityId,
                        "Communities",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Posts_CommunityId",
                "Posts",
                "CommunityId");

            migrationBuilder.CreateIndex(
                "IX_Communities_AuthorId",
                "Communities",
                "AuthorId");

            migrationBuilder.CreateIndex(
                "IX_Communities_Name",
                "Communities",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_CommunityCategories_CategoryId",
                "CommunityCategories",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_CommunityCategories_CommunityId",
                "CommunityCategories",
                "CommunityId");

            migrationBuilder.AddForeignKey(
                "FK_Posts_Communities_CommunityId",
                "Posts",
                "CommunityId",
                "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_Posts_Communities_CommunityId",
                "Posts");

            migrationBuilder.DropTable(
                "CommunityCategories");

            migrationBuilder.DropTable(
                "Communities");

            migrationBuilder.DropIndex(
                "IX_Posts_CommunityId",
                "Posts");

            migrationBuilder.DropColumn(
                "CommunityId",
                "Posts");

            migrationBuilder.DropColumn(
                "ContentType",
                "Posts");

            migrationBuilder.AddColumn<string>(
                "CategoryId",
                "Posts",
                "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                "Name",
                "Categories",
                "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                "AuthorId",
                "Categories",
                "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Description",
                "Categories",
                "ntext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "Image",
                "Categories",
                "nvarchar(max)",
                nullable: true,
                defaultValue: "Category/no-image.jpg");

            migrationBuilder.AlterColumn<string>(
                "Image",
                "AspNetUsers",
                "nvarchar(max)",
                nullable: true,
                defaultValue: "Category/no-image.jpg",
                oldClrType: typeof(string),
                oldNullable: true,
                oldDefaultValue: "Community/no-image.jpg");

            migrationBuilder.CreateIndex(
                "IX_Posts_CategoryId",
                "Posts",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_Categories_AuthorId",
                "Categories",
                "AuthorId");

            migrationBuilder.CreateIndex(
                "IX_Categories_Name",
                "Categories",
                "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                "FK_Categories_AspNetUsers_AuthorId",
                "Categories",
                "AuthorId",
                "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                "FK_Posts_Categories_CategoryId",
                "Posts",
                "CategoryId",
                "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}