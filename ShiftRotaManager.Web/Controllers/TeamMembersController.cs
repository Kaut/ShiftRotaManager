using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Web.Models;
using ShiftRotaManager.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace ShiftRotaManager.Web.Controllers
{
    [Authorize]
    public class TeamMembersController : Controller
    {
        private readonly ITeamMemberService _teamMemberService;
        private readonly IShiftService _shiftService;


        public TeamMembersController(ITeamMemberService teamMemberService, IShiftService shiftService)
        {
            _teamMemberService = teamMemberService;
            _shiftService = shiftService;
        }

        // GET: TeamMembers
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();

            ViewBag.GroupedPreferences = teamMembers.ToDictionary(
                member => member.Id,
                member => member.Preferences
                    .GroupBy(p => p.Shift.Name)
                    .Select(g => $"{g.Key}: {string.Join(", ", g.OrderBy(p => p.DayOfWeek).Select(p => p.DayOfWeek.ToString().Substring(0, 3)))}")
                    .OrderBy(s => s)
                    .ToList()
            );
            return View(teamMembers);
        }

        // GET: TeamMembers/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TeamMemberViewModel
            {
                Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name"),
                Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: TeamMembers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,RoleId,PreferredShifts")] TeamMemberViewModel teamMember)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var member = new TeamMember
                    {
                        FirstName = teamMember.FirstName,
                        LastName = teamMember.LastName,
                        Email = teamMember.Email,
                        RoleId = teamMember.RoleId,
                        Preferences = teamMember.PreferredShifts
                            .Where(p => p.Value.HasValue)
                            .Select(p => new TeamMemberPreference
                            {
                                DayOfWeek = p.Key,
                                ShiftId = p.Value.Value
                            }).ToList()
                    };
                    await _teamMemberService.AddTeamMemberAsync(member);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the team member.");
                }
            }
            teamMember.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", teamMember.RoleId);
            teamMember.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name");
            return View(teamMember);
        }

        // GET: TeamMembers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id.Value);
            if (teamMember == null)
            {
                return NotFound();
            }
            var viewModel = new TeamMemberViewModel
            {
                Id = teamMember.Id,
                Email = teamMember.Email,
                FirstName = teamMember.FirstName,
                LastName = teamMember.LastName,
                RoleId = teamMember.RoleId,
                PreferredShifts = Enum.GetValues(typeof(DayOfWeek))
                    .Cast<DayOfWeek>()
                    .ToDictionary(
                        day => day,
                        day => teamMember.Preferences.FirstOrDefault(p => p.DayOfWeek == day)?.ShiftId
                    ),
                Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", teamMember.RoleId),
                Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: TeamMembers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, 
            [Bind("Id,FirstName,LastName,Email,RoleId,PreferredShifts")] TeamMemberViewModel teamMember)
        {
            if (id != teamMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var shifts = await _shiftService.GetAllShiftsAsync();
                    var member = new TeamMember
                    {
                        Id = teamMember.Id,
                        FirstName = teamMember.FirstName,
                        LastName = teamMember.LastName,
                        Email = teamMember.Email,
                        RoleId = teamMember.RoleId,
                        Preferences = teamMember.PreferredShifts
                            .Where(p => p.Value.HasValue)
                            .Select(p => new TeamMemberPreference
                            {
                                DayOfWeek = p.Key,
                                Shift= shifts.FirstOrDefault(s => s.Id == p.Value.Value),
                                ShiftId = p.Value.Value,
                                TeamMemberId = teamMember.Id
                            }).ToList()
                    };
                    await _teamMemberService.UpdateTeamMemberAsync(member);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the team member.");
                }
            }
            // Repopulate the roles for the dropdown if we return to the view
            teamMember.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", teamMember.RoleId);
            teamMember.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name");
            return View(teamMember);
        }

        // GET: TeamMembers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id.Value);
            if (teamMember == null)
            {
                return NotFound();
            }

            return View(teamMember);
        }

        // POST: TeamMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {

            var teamMember = await _teamMemberService.GetTeamMemberByIdAsync(id);
            if (teamMember == null)
            {
                return NotFound();
            }
            await _teamMemberService.DeleteTeamMemberAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}