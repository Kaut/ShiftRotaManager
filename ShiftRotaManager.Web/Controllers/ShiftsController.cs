using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftRotaManager.Core.Interfaces;
using ShiftRotaManager.Data.Models;
using System;
using System.Threading.Tasks;

namespace ShiftRotaManager.Web.Controllers
{
    [Authorize]
    public class ShiftsController : Controller
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        // GET: Shifts
        public async Task<IActionResult> Index()
        {
            var shifts = await _shiftService.GetAllShiftsAsync();
            return View(shifts);
        }

        // GET: Shifts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shifts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartTime,EndTime,IsNightShift,MinStaffRequired,MaxStaffAllowed")] Shift shift)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate duration (simple example, might need more complex logic for night shifts spanning days)
                    shift.Duration = shift.EndTime - shift.StartTime;
                    if (shift.Duration < TimeSpan.Zero && shift.IsNightShift)
                    {
                        shift.Duration = shift.Duration.Add(new TimeSpan(24, 0, 0)); // Add 24 hours for night shifts crossing midnight
                    }

                    await _shiftService.AddShiftAsync(shift);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the shift.");
                }
            }
            return View(shift);
        }

        // GET: Shifts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _shiftService.GetShiftByIdAsync(id.Value);
            if (shift == null)
            {
                return NotFound();
            }
            return View(shift);
        }

        // POST: Shifts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,StartTime,EndTime,IsNightShift,MinStaffRequired,MaxStaffAllowed")] Shift shift)
        {
            if (id != shift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    shift.Duration = shift.EndTime - shift.StartTime;
                    if (shift.Duration < TimeSpan.Zero && shift.IsNightShift)
                    {
                        shift.Duration = shift.Duration.Add(new TimeSpan(24, 0, 0));
                    }
                    await _shiftService.UpdateShiftAsync(shift);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the shift.");
                }
            }
            return View(shift);
        }

        // GET: Shifts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shift = await _shiftService.GetShiftByIdAsync(id.Value);
            if (shift == null)
            {
                return NotFound();
            }

            return View(shift);
        }

        // POST: Shifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _shiftService.DeleteShiftAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Shifts/CreateVariant/5
        public async Task<IActionResult> CreateVariant(Guid baseShiftId)
        {
            var baseShift = await _shiftService.GetShiftByIdAsync(baseShiftId);
            if (baseShift == null)
            {
                return NotFound();
            }
            ViewBag.BaseShiftName = baseShift.Name;
            return View(new ShiftVariant { BaseShiftId = baseShiftId });
        }

        // POST: Shifts/CreateVariant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVariant([Bind("Name,BaseShiftId,StartTimeOffset,EndTimeOffset")] ShiftVariant variant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _shiftService.AddShiftVariantAsync(variant);
                    return RedirectToAction(nameof(Index)); // Or to a specific shift details page
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while adding the shift variant.");
                }
            }
            var baseShift = await _shiftService.GetShiftByIdAsync(variant.BaseShiftId);
            ViewBag.BaseShiftName = baseShift?.Name;
            return View(variant);
        }
    }
}