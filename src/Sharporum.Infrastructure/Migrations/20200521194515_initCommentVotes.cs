using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sharporum.Infrastructure.Migrations
{
    public partial class initCommentVotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "CommentVotes",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CommentId = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Direction = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentVotes", x => x.Id);
                    table.ForeignKey(
                        "FK_CommentVotes_Comments_CommentId",
                        x => x.CommentId,
                        "Comments",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_CommentVotes_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_CommentVotes_CommentId",
                "CommentVotes",
                "CommentId");

            migrationBuilder.CreateIndex(
                "IX_CommentVotes_UserId",
                "CommentVotes",
                "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "CommentVotes");
        }
    }
}