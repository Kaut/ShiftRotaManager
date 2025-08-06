using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;

namespace ShiftRotaManager.Web.Controllers
{
    public class TeamMembersController : Controller
    {
        private readonly ITeamMemberService _teamMemberService;

        public TeamMembersController(ITeamMemberService teamMemberService)
        {
            _teamMemberService = teamMemberService;
        }

        // GET: TeamMembers
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
            return View(teamMembers);
        }

        // GET: TeamMembers/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name");
            return View();
        }

        // POST: TeamMembers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,RoleId")] TeamMember teamMember, Guid RoleId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _teamMemberService.AddTeamMemberAsync(teamMember, RoleId);
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
            ViewBag.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", RoleId);
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
            // Note: Updating roles would require more complex logic here, e.g., fetching current user roles
            ViewBag.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", teamMember.RoleId);
            return View(teamMember);
        }

        // POST: TeamMembers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FirstName,LastName,Email,RoleId")] TeamMember teamMember)
        {
            if (id != teamMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _teamMemberService.UpdateTeamMemberAsync(teamMember);
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
            ViewBag.Roles = new SelectList(await _teamMemberService.GetAllRolesAsync(), "Id", "Name", teamMember.RoleId);
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