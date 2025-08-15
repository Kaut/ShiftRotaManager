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
        private readonly ILogger<RotasController> _logger;

        public RotasController(IRotaService rotaService,
            IShiftService shiftService,
            ITeamMemberService teamMemberService, ILogger<RotasController> logger)
        {
            _rotaService = rotaService;
            _shiftService = shiftService;
            _teamMemberService = teamMemberService;
            _logger = logger;
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
            var shifts = await _shiftService.GetAllShiftsAsync();
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();

            var viewModel = new CreateRotaViewModel
            {
                Shifts = new SelectList(shifts, "Id", "Name"),
                TeamMembers = new SelectList(teamMembers, "Id", "FullName")
            };

            return View(viewModel);
        }

        // POST: Rotas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRotaViewModel viewModel)
        {
            if (viewModel.EndDate < viewModel.StartDate)
            {
                ModelState.AddModelError("EndDate", "End Date cannot be before the Start Date.");
            }

            if (viewModel.SuggestedRotas == null || !viewModel.SuggestedRotas.Any())
            {
                if (!viewModel.IsOpenShift && viewModel.TeamMemberId == null)
                {
                    ModelState.AddModelError("TeamMemberId", "A team member must be selected for a non-open shift.");
                }

                if (!ModelState.IsValid)
                { 
                    await PopulateViewModelDropdowns(viewModel);
                    return View(viewModel);
                }
            }

            var rotas = new List<Rota>();
            try
            {
                 var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();
                if (viewModel.SuggestedRotas != null && viewModel.SuggestedRotas.Any())
                {
                    // Scenario 1: Create rotas from the suggested assignments
                    foreach (var suggestedRotasByDay in viewModel.SuggestedRotas.Values)
                    {
                        foreach (var suggestedRota in suggestedRotasByDay)
                        {
                            var rota = rotas.FirstOrDefault(x => x.Date == suggestedRota.Date && x.ShiftId == suggestedRota.ShiftId);

                            if (rota == null)
                            {
                                rotas.Add(new Rota
                                {
                                    Date = suggestedRota.Date,
                                    ShiftId = suggestedRota.ShiftId,
                                    TeamMemberId = suggestedRota.TeamMemberId
                                });
                            }
                            else
                            {
                                rota.SelectedPairedTeamMemberIds.Add(suggestedRota.TeamMemberId);
                                rota.PairedTeamMembers = teamMembers.Where(tm => rota.SelectedPairedTeamMemberIds.Contains(tm.Id)).ToList();
                            }
                        }

                    }
                }
                else
                {
                    // Scenario 2: Fallback to manual creation for the date range
                    rotas.Clear();
                    for (var date = viewModel.StartDate; date <= viewModel.EndDate; date = date.AddDays(1))
                    {
                        rotas.Add(new Rota
                        {
                            Date = date,
                            ShiftId = viewModel.ShiftId,
                            TeamMemberId = viewModel.IsOpenShift ? null : viewModel.TeamMemberId,
                            SelectedPairedTeamMemberIds = [.. viewModel.SelectedPairedTeamMemberIds],
                            PairedTeamMembers = teamMembers.Where(tm => viewModel.SelectedPairedTeamMemberIds.Contains(tm.Id)).ToList()
                        });
                    }
                }

                await _rotaService.CreateRotasForDateRangeAsync(rotas);
                _logger.LogInformation("Successfully created {RotaCount} rotas for date range {StartDate} to {EndDate}.", rotas.Count, viewModel.StartDate, viewModel.EndDate);
                return RedirectToAction(nameof(Calendar));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating rotas for date range {StartDate} to {EndDate}.", viewModel.StartDate, viewModel.EndDate);
                ModelState.AddModelError("", "An unexpected error occurred while creating the rotas. Please try again.");
            }
            await PopulateViewModelDropdowns(viewModel);
            return View(viewModel);
        }

        private async Task PopulateViewModelDropdowns(CreateRotaViewModel viewModel)
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            var teamMembers = await _teamMemberService.GetAllTeamMembersAsync();

            viewModel.Shifts = new SelectList(shifts, "Id", "Name");
            viewModel.TeamMembers = new SelectList(teamMembers, "Id", "FullName");
        }

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
            ViewBag.TeamMembers = new SelectList(teamMembers, "Id", "FullName", rota.TeamMemberId);
            ViewBag.PairedTeamMembers = new SelectList(teamMembers, "Id", "FullName", rota.PairedTeamMembers?.Select(x => x.Id));
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
                return RedirectToAction(nameof(Calendar));
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
            ViewBag.TeamMembers = new SelectList(teamMembers, "Id", "FullName", model.TeamMemberId);
            ViewBag.PairedTeamMembers = new SelectList(teamMembers, "Id", "FullName", model.SelectedPairedTeamMemberIds);
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
            return RedirectToAction(nameof(Calendar));
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

        [HttpGet]
        public async Task<IActionResult> GetRecommendedMembers(DateTime startDate, DateTime endDate)
        {
            // Fetch all members once to avoid multiple database calls.
            var allMembers = await _teamMemberService.GetAllTeamMembersAsync();

            var recommendations = new List<object>();
            
            // Loop through each day in the date range.
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                // Get the day of the week for the current date (e.g., "Monday").
                var dayOfWeek = date.DayOfWeek.ToString();

                // Find all members whose preferred days string contains the current day.
                var recommendedMembers = allMembers
                    .Where(m => !string.IsNullOrEmpty(m.PreferredDaysOfWeek) && m.PreferredDaysOfWeek.Contains(dayOfWeek))
                    .Select(m => new {
                        id = m.Id,
                        name = $"{m.FirstName} {m.LastName}",
                        preferredDay = dayOfWeek,
                        preferredShiftId = m.PreferredShift.Id,
                        preferredShiftName = m.PreferredShift != null ? m.PreferredShift.Name : "N/A" })
                    .ToList();

                recommendations.Add(new 
                {
                    date = date.ToShortDateString(),
                    members = recommendedMembers
                });
            }

            // Return the list of recommended members per day as a JSON object.
            return Json(recommendations);
        }
    }
}