using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Data.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure the database has been created.
            // context.Database.EnsureCreated() is typically called by context.Database.Migrate()
            // but can be useful for quick checks in development.

            // Look for any shifts.
            if (context.Shifts.Any())
            {
                return;   // DB has been seeded
            }

            // Seed Roles
            var roles = new Role[]
            {
                new Role { Id = Guid.NewGuid(), Name = "Manager" },
                new Role { Id = Guid.NewGuid(), Name = "Team Lead" },
                new Role { Id = Guid.NewGuid(), Name = "Engineer" },
                new Role { Id = Guid.NewGuid(), Name = "Support" }
            };
            foreach (Role r in roles)
            {
                context.Roles.Add(r);
            }
            context.SaveChanges();

            var managerRole = context.Roles.Single(r => r.Name == "Manager");
            var engineerRole = context.Roles.Single(r => r.Name == "Engineer");
            var supportRole = context.Roles.Single(r => r.Name == "Support");

            // Seed Team Members
            var teamMembers = new TeamMember[]
            {
                new() { Id = Guid.NewGuid(), FirstName = "Kautilya", LastName = "Shukla", Email = "kautilya.shukla@example.com", RoleId = engineerRole.Id },
                new() { Id = Guid.NewGuid(), FirstName = "Bhromor", LastName = "Gayen", Email = "bhromor.gayen@example.com", RoleId = engineerRole.Id },
                new() { Id = Guid.NewGuid(), FirstName = "Mikey", LastName = "Moore", Email = "mikey.moore@example.com", RoleId = managerRole.Id },
                new() { Id = Guid.NewGuid(), FirstName = "Abdul", LastName = "Rauf", Email = "abdul.rauf@example.com", RoleId = engineerRole.Id }
            };
            foreach (TeamMember tm in teamMembers)
            {
                context.TeamMembers.Add(tm);
            }
            context.SaveChanges();

            // Seed Shifts
            var shifts = new Shift[]
            {
                new() { Id = Guid.NewGuid(), Name = "Morning Shift", StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(14, 0, 0), IsNightShift = false },
                new() { Id = Guid.NewGuid(), Name = "Day Shift", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(17, 0, 0), IsNightShift = false },
                new() { Id = Guid.NewGuid(), Name = "Evening Shift", StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0), IsNightShift = false },
                new() { Id = Guid.NewGuid(), Name = "Night Shift", StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(6, 0, 0), IsNightShift = true } // Crosses midnight
            };
            foreach (Shift s in shifts)
            {
                context.Shifts.Add(s);
            }
            context.SaveChanges();

            // Seed Rotas (some open, some assigned, some paired)
            var rotas = new Rota[]
            {
                // Assigned shifts
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(1), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Mikey").Id, Status = RotaStatus.Assigned },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(1), ShiftId = shifts.Single(s => s.Name == "Evening Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Bhromor").Id, Status = RotaStatus.Assigned },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(2), ShiftId = shifts.Single(s => s.Name == "Morning Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Abdul").Id, Status = RotaStatus.Assigned },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(2), ShiftId = shifts.Single(s => s.Name == "Night Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Kautilya").Id, Status = RotaStatus.Assigned },

                // Open shifts
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(3), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, Status = RotaStatus.Open },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(3), ShiftId = shifts.Single(s => s.Name == "Morning Shift").Id, Status = RotaStatus.Open },

                // Paired shift
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(4), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Mikey").Id, PairedTeamMemberId = teamMembers.Single(tm => tm.FirstName == "Bhromor").Id, Status = RotaStatus.Assigned }
            };
            foreach (Rota r in rotas)
            {
                context.Rotas.Add(r);
            }
            context.SaveChanges();
        }
    }
}