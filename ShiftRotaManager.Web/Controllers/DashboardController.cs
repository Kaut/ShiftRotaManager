// --- ShiftRotaManager.Web/Controllers/DashboardController.cs ---
using Microsoft.AspNetCore.Mvc;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ShiftRotaManager.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ITeamMemberService _teamMemberService;
        private readonly IShiftService _shiftService;
        // private readonly IRoleService _roleService;
        private readonly IRotaService _rotaService;

        public DashboardController(
            ITeamMemberService teamMemberService,
            IShiftService shiftService,            
            IRotaService rotaService)//IRoleService roleService,
        {
            _teamMemberService = teamMemberService;
            _shiftService = shiftService;
            // _roleService = roleService;
            _rotaService = rotaService;
        }

        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
            var shifts = await _shiftService.GetAllShiftsAsync();
            // var roles = await _roleService.GetAllRolesAsync();
            var rotas = await _rotaService.GetAllRotasAsync();

            ViewBag.TeamMemberCount = teamMembers.Count();
            ViewBag.ShiftCount = shifts.Count();
            // ViewBag.RoleCount = roles.Count();
            ViewBag.OpenShiftCount = rotas.Count(r => r.Status == RotaStatus.Open);
            ViewBag.AssignedShiftCount = rotas.Count(r => r.Status == RotaStatus.Assigned);

            return View();
        }
    }
}