using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFollowerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_UserId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_FriendId",
                table: "Friendship");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendship_AspNetUsers_UserId",
                table: "Friendship");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowerUserId",
                table: "Followers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship");

            migrationBuilder.DropColumn(
                name: "FollowerUserId",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Followers");

            migrationBuilder.RenameTable(
                name: "Friendship",
                newName: "Friendships");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Followers",
                newName: "FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_Followers_UserId",
                table: "Followers",
                newName: "IX_Followers_FollowingId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendship_FriendId",
                table: "Friendships",
                newName: "IX_Friendships_FriendId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Followers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Followers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                table: "Followers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers",
                column: "FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers",
                column: "FollowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowingId",
                table: "Followers",
                column: "FollowingId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Followers_AspNetUsers_FollowingId",
                table: "Followers");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_FriendId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_UserId",
                table: "Friendships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Followers",
                table: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Followers_FollowerId",
                table: "Followers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friendships",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Followers");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Followers");

            migrationBuilder.RenameTable(
                name: "Friendships",
                newName: "Friendship");

            migrationBuilder.RenameColumn(
                name: "FollowingId",
                table: "Followers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Followers_FollowingId",
                table: "Followers",
                newName: "IX_Followers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendship",
                newName: "IX_Friendship_FriendId");

            migrationBuilder.AddColumn<string>(
                name: "FollowerUserId",
                table: "Followers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Followers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Followers",
                table: "Followers",
                column: "FollowerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friendship",
                table: "Friendship",
                columns: new[] { "UserId", "FriendId" });

            migrationBuilder.CreateIndex(
                name: "IX_Followers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_FollowerUserId",
                table: "Followers",
                column: "FollowerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Followers_AspNetUsers_UserId",
                table: "Followers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
    }
}
