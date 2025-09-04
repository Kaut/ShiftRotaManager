using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftRotaManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamMemberPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Shifts_ShiftId",
                table: "TeamMembers");

            migrationBuilder.DropIndex(
                name: "IX_TeamMembers_ShiftId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "PreferredDaysOfWeek",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "TeamMembers");

            migrationBuilder.CreateTable(
                name: "TeamMemberPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ShiftId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMemberPreferences_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMemberPreferences_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberPreferences_ShiftId",
                table: "TeamMemberPreferences",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberPreferences_TeamMemberId",
                table: "TeamMemberPreferences",
                column: "TeamMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMemberPreferences");

            migrationBuilder.AddColumn<string>(
                name: "PreferredDaysOfWeek",
                table: "TeamMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "TeamMembers",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_ShiftId",
                table: "TeamMembers",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Shifts_ShiftId",
                table: "TeamMembers",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
