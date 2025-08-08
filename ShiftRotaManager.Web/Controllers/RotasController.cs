using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using ShiftRotaManager.Web.Models;

namespace ShiftRotaManager.Web.Controllers
{
    public class RotasController : Controller
    {
        private readonly IRotaService _rotaService;
        private readonly IShiftService _shiftService;
        private readonly ITeamMemberService _teamMemberService;

        public RotasController(IRotaService rotaService, IShiftService shiftService, ITeamMemberService teamMemberService)
        {
            _rotaService = rotaService;
            _shiftService = shiftService;
            _teamMemberService = teamMemberService;
        }

        // GET: Rotas
        public async Task<IActionResult> Index()
        {
            var rotas = await _rotaService.GetAllRotasAsync();
            return View(rotas);
        }

        // GET: Rotas/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name");
            // Allow null for TeamMemberId to create open shifts
            ViewBag.TeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName");
            ViewBag.PairedTeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName");
            return View();
        }

        // POST: Rotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,ShiftId,TeamMemberId,SelectedPairedTeamMemberIds")] RotaViewModel model)
        {
            // ModelState.IsValid check usually happens here, but for simplicity in POC,
            // we'll rely on service layer validation for now.
            // For production, add Data Annotations to Rota model and check ModelState.IsValid

            try
            {
                var rota = new Rota
                {
                    Date = model.Date,
                    ShiftId = model.ShiftId,
                    TeamMemberId = model.TeamMemberId,
                    PairedTeamMembers = await _teamMemberService.GetTeamMembersByIdsAsync(model.SelectedPairedTeamMemberIds)
                };

                await _rotaService.AddRotaAsync(rota);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while adding the rota.");
            }

            ViewBag.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name", model.ShiftId);
            ViewBag.TeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName", model.TeamMemberId);
            ViewBag.PairedTeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName", model.SelectedPairedTeamMemberIds);
            return View(model);
        }

        // GET: Rotas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rota = await _rotaService.GetRotaByIdAsync(id.Value);
            if (rota == null)
            {
                return NotFound();
            }
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
            ViewBag.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name", rota.ShiftId);
            ViewBag.TeamMembers = new SelectList(teamMembers, "Id", "FirstName", rota.TeamMemberId);
            ViewBag.PairedTeamMembers = new SelectList(teamMembers, "Id", "FirstName", rota.PairedTeamMembers?.Select(x => x.Id));
            return View(rota);
        }

        // POST: Rotas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,ShiftId,TeamMemberId,SelectedPairedTeamMemberIds")] RotaViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            // ModelState.IsValid check usually happens here
            try
            {
                var rota = await _rotaService.GetRotaByIdAsync(id);
                if (rota == null)
                {
                    return NotFound();
                }

                // Update properties
                rota.Date = model.Date;
                rota.ShiftId = model.ShiftId;
                rota.TeamMemberId = model.TeamMemberId;
                rota.PairedTeamMembers = await _teamMemberService.GetTeamMembersByIdsAsync(model.SelectedPairedTeamMemberIds);


                await _rotaService.UpdateRotaAsync(rota);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while updating the rota.");
            }

            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
            ViewBag.Shifts = new SelectList(await _shiftService.GetAllShiftsAsync(), "Id", "Name", model.ShiftId);
            ViewBag.TeamMembers = new SelectList(teamMembers, "Id", "FirstName", model.TeamMemberId);
            ViewBag.PairedTeamMembers = new SelectList( teamMembers, "Id", "FirstName", model.SelectedPairedTeamMemberIds);
            return View(model);
        }

        // GET: Rotas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rota = await _rotaService.GetRotaByIdAsync(id.Value);
            if (rota == null)
            {
                return NotFound();
            }

            return View(rota);
        }

        // POST: Rotas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _rotaService.DeleteRotaAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Rotas/AssignOpenShift/5
        public async Task<IActionResult> AssignOpenShift(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rota = await _rotaService.GetRotaByIdAsync(id.Value);
            if (rota == null || rota.Status != RotaStatus.Open)
            {
                return NotFound();
            }

            ViewBag.TeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName");
            return View(rota);
        }

        // POST: Rotas/AssignOpenShift/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignOpenShift(Guid id, Guid teamMemberId)
        {
            try
            {
                await _rotaService.AssignOpenShiftAsync(id, teamMemberId);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while assigning the shift.");
            }

            var rota = await _rotaService.GetRotaByIdAsync(id);
            ViewBag.TeamMembers = new SelectList(await _teamMemberService.GetAllTeamMembersAsync(), "Id", "FirstName", teamMemberId);
            return View(rota);
        }

        // GET: Rotas/Calendar - New action for the calendar view
        public IActionResult Calendar()
        {
            ViewData["Title"] = "Rota Calendar";
            return View();
        }

        // GET: Rotas/GetCalendarRotas - API endpoint for FullCalendar
        [HttpGet]
        public async Task<IActionResult> GetCalendarRotas(DateTime start, DateTime end)
        {
            var rotas = await _rotaService.GetRotasForCalendarAsync(start, end);

            var events = rotas.Select(r =>
            {
                var title = $"{r.Shift.Name}";
                if (r.TeamMember != null)
                {
                    title += $" - {r.TeamMember.FirstName}";
                }
                else
                {
                    title += " (Open)";
                }

                foreach (var p in r.PairedTeamMembers ?? [])
                {
                    title += $" w/ {p.FirstName} {p.LastName}";
                }

                // FullCalendar expects 'start' and 'end' in ISO 8601 format
                // Combine Date with Shift's StartTime and EndTime
                var eventStart = r.Date.Date.Add(r.Shift.StartTime);
                var eventEnd = r.Date.Date.Add(r.Shift.EndTime);

                // Handle night shifts that cross midnight
                if (r.Shift.IsNightShift && r.Shift.EndTime <= r.Shift.StartTime)
                {
                    eventEnd = eventEnd.AddDays(1);
                }

                string backgroundColor = "#007bff"; // Default blue for assigned
                string textColor = "#ffffff"; // Default white text
                if (r.Status == RotaStatus.Open)
                {
                    backgroundColor = "#ffc107"; // Yellow for open
                    textColor = "#343a40"; // Dark text for yellow background
                }
                else if (r.Status == RotaStatus.Leave || r.Status == RotaStatus.Illness)
                {
                    backgroundColor = "#dc3545"; // Red for leave/illness
                }
                else if (r.PairedTeamMembers != null && r.PairedTeamMembers.Count > 0)
                {
                    backgroundColor = "#28a745"; // Green for paired
                }


                return new
                {
                    id = r.Id,
                    title = title,
                    start = eventStart.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = eventEnd.ToString("yyyy-MM-ddTHH:mm:ss"),
                    allDay = false, // Shifts usually have specific times
                    backgroundColor = backgroundColor,
                    textColor = textColor,
                    url = $"/Rotas/Edit/{r.Id}" // Link to edit rota
                };
            }).ToList();

            return Json(events);
        }
    }
}