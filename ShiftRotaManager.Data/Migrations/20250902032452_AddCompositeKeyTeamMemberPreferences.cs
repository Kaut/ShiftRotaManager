using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftRotaManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeKeyTeamMemberPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMemberPreferences",
                table: "TeamMemberPreferences");

            migrationBuilder.DropIndex(
                name: "IX_TeamMemberPreferences_TeamMemberId",
                table: "TeamMemberPreferences");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMemberPreferences",
                table: "TeamMemberPreferences",
                columns: new[] { "TeamMemberId", "DayOfWeek", "ShiftId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMemberPreferences",
                table: "TeamMemberPreferences");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMemberPreferences",
                table: "TeamMemberPreferences",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberPreferences_TeamMemberId",
                table: "TeamMemberPreferences",
                column: "TeamMemberId");
        }
    }
}
