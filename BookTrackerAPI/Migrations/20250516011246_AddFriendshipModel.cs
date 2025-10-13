using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendshipModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_FriendId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friendship");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendship",
                newName: "IX_Friendship_FriendId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Friendship",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FollowerUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.FollowerId);
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_FollowerUserId",
                        column: x => x.FollowerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Followers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Followers_UserId",
                table: "Followers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_FriendId",
                table: "Friendship",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendship_AspNetUsers_UserId",
                table: "Friendship",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_FriendId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_UserId",
                table: "Friendship");

            migrationBuilder.DropTable(
                name: "Followers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friendship");

            migrationBuilder.RenameTable(
                name: "Friendship",
                newName: "Friendships");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendships",
                newName: "IX_Friendships_FriendId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_FriendId",
                table: "Friendships",
                column: "FriendId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId",
                table: "Friendships",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
