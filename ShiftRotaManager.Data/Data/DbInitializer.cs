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
                new Role { Id = Guid.NewGuid(), Name = "Engineer" },
            };
            foreach (Role r in roles)
            {
                context.Roles.Add(r);
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

            var managerRole = context.Roles.Single(r => r.Name == "Manager");
            var engineerRole = context.Roles.Single(r => r.Name == "Engineer");

            var PreferredDaysOfWeek1 = "Sunday,Monday,Tuesday,Wednesday,Thursday";
            var PreferredDaysOfWeek2 = "Tuesday,Wednesday,Thursday,Friday,Saturday";

            // Seed Team Members
            var teamMembers = new TeamMember[]
            {
                new() { Id = Guid.NewGuid(), FirstName = "Kautilya", LastName = "Shukla", Email = "kautilya.shukla@example.com", RoleId = engineerRole.Id },
                new() { Id = Guid.NewGuid(), FirstName = "Bhromor", LastName = "Gayen", Email = "bhromor.gayen@example.com", RoleId = engineerRole.Id },
                new() { Id = Guid.NewGuid(), FirstName = "Mikey", LastName = "Moore", Email = "mikey.moore@example.com", RoleId = managerRole.Id},
                new() { Id = Guid.NewGuid(), FirstName = "Abdul", LastName = "Rauf", Email = "abdul.rauf@example.com", RoleId = engineerRole.Id},
                new() { Id = Guid.NewGuid(), FirstName = "Abdul", LastName = "Razaq", Email = "abdul.razaq@example.com", RoleId = engineerRole.Id},
                new() { Id = Guid.NewGuid(), FirstName = "Sridhar", LastName = "Konda", Email = "sridhar.konda@example.com", RoleId = engineerRole.Id},
                new() { Id = Guid.NewGuid(), FirstName = "Udaya", LastName = "Arumugam", Email = "udaya.arumugam@example.com", RoleId = engineerRole.Id}
             };
            foreach (TeamMember tm in teamMembers)
            {
                context.TeamMembers.Add(tm);
            }
            context.SaveChanges();


            // Seed Team Member Preferences
            //
            var preferences = new TeamMemberPreference[]
            {
                new() { Id = Guid.NewGuid(), TeamMemberId = teamMembers[0].Id, DayOfWeek = DayOfWeek.Sunday, ShiftId = shifts[3].Id },
                new() { Id = Guid.NewGuid(), TeamMemberId = teamMembers[0].Id, DayOfWeek = DayOfWeek.Monday, ShiftId = shifts[2].Id },
                new() { Id = Guid.NewGuid(), TeamMemberId = teamMembers[0].Id, DayOfWeek = DayOfWeek.Tuesday, ShiftId = shifts[3].Id },
                new() { Id = Guid.NewGuid(), TeamMemberId = teamMembers[0].Id, DayOfWeek = DayOfWeek.Wednesday, ShiftId = shifts[1].Id },
                new() { Id = Guid.NewGuid(), TeamMemberId = teamMembers[0].Id, DayOfWeek = DayOfWeek.Thursday, ShiftId = shifts[0].Id }

            };

            foreach (var p in preferences)
            { 
                context.TeamMemberPreferences.Add(p);
            }
            context.SaveChanges();

            // Seed Rotas (some open, some assigned, some paired)
            var rotas = new Rota[]
            {
                // Assigned shifts
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(1), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Mikey").Id, Status = RotaStatus.Open },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(1), ShiftId = shifts.Single(s => s.Name == "Evening Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Bhromor").Id, Status = RotaStatus.Open },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(2), ShiftId = shifts.Single(s => s.Name == "Morning Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.LastName == "Rauf").Id, Status = RotaStatus.Open },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(2), ShiftId = shifts.Single(s => s.Name == "Night Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Kautilya").Id, Status = RotaStatus.Open },

                // Open shifts
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(3), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, Status = RotaStatus.Open },
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(3), ShiftId = shifts.Single(s => s.Name == "Morning Shift").Id, Status = RotaStatus.Open },

                // Paired shift
                new() { Id = Guid.NewGuid(), Date = DateTime.Today.AddDays(4), ShiftId = shifts.Single(s => s.Name == "Day Shift").Id, TeamMemberId = teamMembers.Single(tm => tm.FirstName == "Mikey").Id, PairedTeamMembers = teamMembers.Where(tm => tm.FirstName.Contains("r")).ToList(), Status = RotaStatus.Assigned }
            };
            foreach (Rota r in rotas)
            {
                context.Rotas.Add(r);
            }
            context.SaveChanges();

            var pairedRotaMembers = new RotaPairedTeamMember[]
            {
                new ()
                {
                    RotaId = rotas[1].Id,
                    TeamMemberId = teamMembers[0].Id
                },
                new ()
                {
                    RotaId = rotas[2].Id,
                    TeamMemberId = teamMembers[1].Id
                },
                new ()
                {
                    RotaId = rotas[3].Id,
                    TeamMemberId = teamMembers[2].Id
                }                
            };
        }
    }
}