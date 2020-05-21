using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Violetum.Infrastructure.Migrations
{
    public partial class initPostVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "PostVotes",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Direction = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostVotes", x => x.Id);
                    table.ForeignKey(
                        "FK_PostVotes_Posts_PostId",
                        x => x.PostId,
                        "Posts",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_PostVotes_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_PostVotes_PostId",
                "PostVotes",
                "PostId");

            migrationBuilder.CreateIndex(
                "IX_PostVotes_UserId",
                "PostVotes",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "PostVotes");
        }
    }
}