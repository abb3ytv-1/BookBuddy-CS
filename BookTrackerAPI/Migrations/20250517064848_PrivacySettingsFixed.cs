using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTrackerAPI.Migrations
{
    /// <inheritdoc />
    public partial class PrivacySettingsFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivacySettings",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    RequireFollowApproval = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivacySettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_PrivacySettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivacySettings");
        }
    }
}
