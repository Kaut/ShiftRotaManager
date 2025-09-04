using Microsoft.EntityFrameworkCore;
using ShiftRotaManager.Data.Models;
using System;
using System.Globalization;

namespace ShiftRotaManager.Data.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<ShiftVariant> ShiftVariants { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<TeamMemberPreference> TeamMemberPreferences { get; set; } = null!;
        public DbSet<Rota> Rotas { get; set; } = null!;
        public DbSet<Rule> Rules { get; set; } = null!;
        public DbSet<CoverageHour> CoverageHours { get; set; } = null!;
        public DbSet<AnnualLeave> AnnualLeaves { get; set; } = null!;
        public DbSet<TimeOffInLieu> TimeOffInLieu { get; set; } = null!;
        public DbSet<Overtime> OvertimeRecords { get; set; } = null!;
        public DbSet<Illness> IllnessRecords { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Role)
                .WithMany() // Or .WithMany(r => r.TeamMembers) if Role had a collection of TeamMembers
                .HasForeignKey(tm => tm.RoleId)
                .IsRequired(); // RoleId is required

            modelBuilder.Entity<ShiftVariant>()
                .HasOne(sv => sv.BaseShift)
                .WithMany(s => s.Variants)
                .HasForeignKey(sv => sv.BaseShiftId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete if base shift has variants

            modelBuilder.Entity<Rota>()
                .HasOne(r => r.Shift)
                .WithMany(s => s.Rotas)
                .HasForeignKey(r => r.ShiftId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a shift if it's used in a rota

            modelBuilder.Entity<Rota>()
                .HasOne(r => r.TeamMember)
                .WithMany(tm => tm.Rotas)
                .HasForeignKey(r => r.TeamMemberId);

            modelBuilder.Entity<Rota>()
                .HasMany(r => r.PairedTeamMembers)
                .WithMany() // No direct collection on TeamMember for PairedTeamMember
                .UsingEntity<Dictionary<string, object>>("RotaPairedTeamMember",
                        j => j.HasOne<TeamMember>().WithMany().HasForeignKey("TeamMemberId").OnDelete(DeleteBehavior.Restrict),
                        j => j.HasOne<Rota>().WithMany().HasForeignKey("RotaId").OnDelete(DeleteBehavior.Cascade));

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.TeamMember)
                .WithMany(tm => tm.UserRoles)
                .HasForeignKey(ur => ur.TeamMemberId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<TeamMemberPreference>()
                .HasKey(p => new { p.TeamMemberId, p.DayOfWeek, p.ShiftId});
            // Seed initial roles
            //SeedData(modelBuilder); EF SQL Server
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"), Name = "Manager" },
                new Role { Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"), Name = "Engineer" },
                new Role { Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"), Name = "Reader" }
            );

            // Seed some sample shifts (from previous response)
            modelBuilder.Entity<Shift>().HasData(
                new Shift
                {
                    Id = Guid.Parse("B0000000-0000-0000-0000-000000000001"),
                    Name = "Morning Shift",
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    Duration = new TimeSpan(8, 0, 0),
                    IsNightShift = false,
                    MinStaffRequired = 2,
                    MaxStaffAllowed = 5
                },
                new Shift
                {
                    Id = Guid.Parse("B0000000-0000-0000-0000-000000000002"),
                    Name = "Afternoon Shift",
                    StartTime = new TimeSpan(14, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    Duration = new TimeSpan(8, 0, 0),
                    IsNightShift = false,
                    MinStaffRequired = 2,
                    MaxStaffAllowed = 5
                },
                new Shift
                {
                    Id = Guid.Parse("B0000000-0000-0000-0000-000000000003"),
                    Name = "Night Shift",
                    StartTime = new TimeSpan(22, 0, 0),
                    EndTime = new TimeSpan(6, 0, 0), // Next day
                    Duration = new TimeSpan(8, 0, 0),
                    IsNightShift = true,
                    MinStaffRequired = 1,
                    MaxStaffAllowed = 3
                }
            );

            // Seed some sample team members and assign them roles
            var adminRoleId = Guid.Parse("A0000000-0000-0000-0000-000000000001");
            var userRoleId = Guid.Parse("A0000000-0000-0000-0000-000000000002");

            var teamMember1Id = Guid.Parse("D0000000-0000-0000-0000-000000000001");
            var teamMember2Id = Guid.Parse("D0000000-0000-0000-0000-000000000002");
            var teamMember3Id = Guid.Parse("D0000000-0000-0000-0000-000000000003");
            var teamMember4Id = Guid.Parse("D0000000-0000-0000-0000-000000000004"); // New starter

            var userRole1Id = Guid.Parse("F0000000-0000-0000-0000-000000000001");
            var userRole2Id = Guid.Parse("F0000000-0000-0000-0000-000000000002");
            var userRole3Id = Guid.Parse("F0000000-0000-0000-0000-000000000003");
            var userRole4Id = Guid.Parse("F0000000-0000-0000-0000-000000000004");

            modelBuilder.Entity<TeamMember>().HasData(
                new TeamMember { Id = teamMember1Id, FirstName = "Alice", LastName = "Smith", Email = "alice.smith@example.com" },
                new TeamMember { Id = teamMember2Id, FirstName = "Bob", LastName = "Johnson", Email = "bob.j@example.com" },
                new TeamMember { Id = teamMember3Id, FirstName = "Charlie", LastName = "Brown", Email = "charlie.b@example.com" },
                new TeamMember { Id = teamMember4Id, FirstName = "David", LastName = "Lee", Email = "david.l@example.com" } // New starter
            );

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole { Id = userRole1Id, TeamMemberId = teamMember1Id, RoleId = adminRoleId },
                new UserRole { Id = userRole2Id, TeamMemberId = teamMember2Id, RoleId = userRoleId },
                new UserRole { Id = userRole3Id, TeamMemberId = teamMember3Id, RoleId = userRoleId },
                new UserRole { Id = userRole4Id, TeamMemberId = teamMember4Id, RoleId = userRoleId }
            );

            // Seed some sample rotas
            var morningShiftId = Guid.Parse("B0000000-0000-0000-0000-000000000001");
            var afternoonShiftId = Guid.Parse("B0000000-0000-0000-0000-000000000002");
            var nightShiftId = Guid.Parse("B0000000-0000-0000-0000-000000000003");

            // Seed some sample rota ids
            var rota1Id = Guid.Parse("E0000000-0000-0000-0000-000000000001");
            var rota2Id = Guid.Parse("E0000000-0000-0000-0000-000000000002");
            var rota3Id = Guid.Parse("E0000000-0000-0000-0000-000000000003");
            var rota4Id = Guid.Parse("E0000000-0000-0000-0000-000000000004");
            var rota5Id = Guid.Parse("E0000000-0000-0000-0000-000000000005");
            var rota6Id = Guid.Parse("E0000000-0000-0000-0000-000000000006");

            var cultureInfo = new CultureInfo("en-GB");

            modelBuilder.Entity<Rota>().HasData(
                // Assigned shifts
                new Rota { Id = rota1Id, Date = DateTime.Parse("05/08/2025 00:00:00", cultureInfo), ShiftId = morningShiftId, TeamMemberId = teamMember1Id, Status = RotaStatus.Assigned },
                new Rota { Id = rota2Id, Date = DateTime.Parse("05/08/2025 00:00:00", cultureInfo), ShiftId = morningShiftId, TeamMemberId = teamMember2Id, Status = RotaStatus.Assigned },
                new Rota { Id = rota3Id, Date = DateTime.Parse("06/08/2025 00:00:00", cultureInfo), ShiftId = afternoonShiftId, TeamMemberId = teamMember3Id, Status = RotaStatus.Assigned },
                new Rota { Id = rota4Id, Date = DateTime.Parse("07/08/2025 00:00:00", cultureInfo), ShiftId = nightShiftId, TeamMemberId = teamMember1Id, Status = RotaStatus.Assigned },
                // Open shift
                new Rota { Id = rota5Id, Date = DateTime.Parse("08/08/2025 00:00:00", cultureInfo), ShiftId = morningShiftId, TeamMemberId = null, Status = RotaStatus.Open },
                // Paired shift (Alice training David)
                new Rota { Id = rota6Id, Date = DateTime.Parse("08/08/2025 00:00:00", cultureInfo), ShiftId = afternoonShiftId, TeamMemberId = teamMember1Id, PairedTeamMembers = [] , Status = RotaStatus.Assigned }
            );
        }
    }
}