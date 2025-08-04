using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShiftRotaManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoverageHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RequiredStaffCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverageHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsNightShift = table.Column<bool>(type: "bit", nullable: false),
                    MinStaffRequired = table.Column<int>(type: "int", nullable: false),
                    MaxStaffAllowed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTimeOffset = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTimeOffset = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftVariants_Shifts_BaseShiftId",
                        column: x => x.BaseShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnnualLeaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnualLeaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnualLeaves_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IllnessRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllnessRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IllnessRecords_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OvertimeRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hours = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OvertimeRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OvertimeRecords_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rotas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PairedTeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rotas_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rotas_TeamMembers_PairedTeamMemberId",
                        column: x => x.PairedTeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rotas_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TimeOffInLieu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursAccrued = table.Column<double>(type: "float", nullable: false),
                    HoursUsed = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeOffInLieu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeOffInLieu_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_TeamMembers_TeamMemberId",
                        column: x => x.TeamMemberId,
                        principalTable: "TeamMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "Admin" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "User" },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "Reader" }
                });

            migrationBuilder.InsertData(
                table: "Shifts",
                columns: new[] { "Id", "Duration", "EndTime", "IsNightShift", "MaxStaffAllowed", "MinStaffRequired", "Name", "StartTime" },
                values: new object[,]
                {
                    { new Guid("b0000000-0000-0000-0000-000000000001"), new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 14, 0, 0, 0), false, 5, 2, "Morning Shift", new TimeSpan(0, 6, 0, 0, 0) },
                    { new Guid("b0000000-0000-0000-0000-000000000002"), new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 22, 0, 0, 0), false, 5, 2, "Afternoon Shift", new TimeSpan(0, 14, 0, 0, 0) },
                    { new Guid("b0000000-0000-0000-0000-000000000003"), new TimeSpan(0, 8, 0, 0, 0), new TimeSpan(0, 6, 0, 0, 0), true, 3, 1, "Night Shift", new TimeSpan(0, 22, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "TeamMembers",
                columns: new[] { "Id", "Email", "FirstName", "LastName" },
                values: new object[,]
                {
                    { new Guid("d0000000-0000-0000-0000-000000000001"), "alice.smith@example.com", "Alice", "Smith" },
                    { new Guid("d0000000-0000-0000-0000-000000000002"), "bob.j@example.com", "Bob", "Johnson" },
                    { new Guid("d0000000-0000-0000-0000-000000000003"), "charlie.b@example.com", "Charlie", "Brown" },
                    { new Guid("d0000000-0000-0000-0000-000000000004"), "david.l@example.com", "David", "Lee" }
                });

            migrationBuilder.InsertData(
                table: "Rotas",
                columns: new[] { "Id", "Date", "PairedTeamMemberId", "ShiftId", "Status", "TeamMemberId" },
                values: new object[,]
                {
                    { new Guid("e0000000-0000-0000-0000-000000000001"), new DateTime(2025, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("b0000000-0000-0000-0000-000000000001"), 0, new Guid("d0000000-0000-0000-0000-000000000001") },
                    { new Guid("e0000000-0000-0000-0000-000000000002"), new DateTime(2025, 8, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("b0000000-0000-0000-0000-000000000001"), 0, new Guid("d0000000-0000-0000-0000-000000000002") },
                    { new Guid("e0000000-0000-0000-0000-000000000003"), new DateTime(2025, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("b0000000-0000-0000-0000-000000000002"), 0, new Guid("d0000000-0000-0000-0000-000000000003") },
                    { new Guid("e0000000-0000-0000-0000-000000000004"), new DateTime(2025, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("b0000000-0000-0000-0000-000000000003"), 0, new Guid("d0000000-0000-0000-0000-000000000001") },
                    { new Guid("e0000000-0000-0000-0000-000000000005"), new DateTime(2025, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, new Guid("b0000000-0000-0000-0000-000000000001"), 1, null },
                    { new Guid("e0000000-0000-0000-0000-000000000006"), new DateTime(2025, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d0000000-0000-0000-0000-000000000004"), new Guid("b0000000-0000-0000-0000-000000000002"), 0, new Guid("d0000000-0000-0000-0000-000000000001") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "TeamMemberId" },
                values: new object[,]
                {
                    { new Guid("f0000000-0000-0000-0000-000000000001"), new Guid("a0000000-0000-0000-0000-000000000001"), new Guid("d0000000-0000-0000-0000-000000000001") },
                    { new Guid("f0000000-0000-0000-0000-000000000002"), new Guid("a0000000-0000-0000-0000-000000000002"), new Guid("d0000000-0000-0000-0000-000000000002") },
                    { new Guid("f0000000-0000-0000-0000-000000000003"), new Guid("a0000000-0000-0000-0000-000000000002"), new Guid("d0000000-0000-0000-0000-000000000003") },
                    { new Guid("f0000000-0000-0000-0000-000000000004"), new Guid("a0000000-0000-0000-0000-000000000002"), new Guid("d0000000-0000-0000-0000-000000000004") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnualLeaves_TeamMemberId",
                table: "AnnualLeaves",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_IllnessRecords_TeamMemberId",
                table: "IllnessRecords",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_OvertimeRecords_TeamMemberId",
                table: "OvertimeRecords",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotas_PairedTeamMemberId",
                table: "Rotas",
                column: "PairedTeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotas_ShiftId",
                table: "Rotas",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Rotas_TeamMemberId",
                table: "Rotas",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftVariants_BaseShiftId",
                table: "ShiftVariants",
                column: "BaseShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeOffInLieu_TeamMemberId",
                table: "TimeOffInLieu",
                column: "TeamMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_TeamMemberId",
                table: "UserRoles",
                column: "TeamMemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnualLeaves");

            migrationBuilder.DropTable(
                name: "CoverageHours");

            migrationBuilder.DropTable(
                name: "IllnessRecords");

            migrationBuilder.DropTable(
                name: "OvertimeRecords");

            migrationBuilder.DropTable(
                name: "Rotas");

            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "ShiftVariants");

            migrationBuilder.DropTable(
                name: "TimeOffInLieu");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "TeamMembers");
        }
    }
}
