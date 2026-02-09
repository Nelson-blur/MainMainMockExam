using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mock_Exam_Work.Data;
using Mock_Exam_Work.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mock_Exam_Work.Controllers
{
    
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(string searchCity, float? minRate, float? maxRate, int? minCapacity, bool? availableOnly)
        {
            var rooms = _context.Rooms.AsQueryable();

            // Filter by city
            if (!string.IsNullOrEmpty(searchCity))
            {
                rooms = rooms.Where(r => r.City.Contains(searchCity));
            }

            // Filter by minimum rate
            if (minRate.HasValue)
            {
                rooms = rooms.Where(r => r.HourlyRate >= minRate.Value);
            }

            // Filter by maximum rate
            if (maxRate.HasValue)
            {
                rooms = rooms.Where(r => r.HourlyRate <= maxRate.Value);
            }

            // Filter by minimum capacity
            if (minCapacity.HasValue)
            {
                rooms = rooms.Where(r => r.Capacity >= minCapacity.Value);
            }

            // Filter by availability
            if (availableOnly.HasValue && availableOnly.Value)
            {
                rooms = rooms.Where(r => r.IsAvailable);
            }

            // Pass filter values to view for form persistence
            ViewBag.SearchCity = searchCity;
            ViewBag.MinRate = minRate;
            ViewBag.MaxRate = maxRate;
            ViewBag.MinCapacity = minCapacity;
            ViewBag.AvailableOnly = availableOnly;

            return View(await rooms.OrderBy(r => r.City).ThenBy(r => r.HourlyRate).ToListAsync());
        }


        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rooms = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomsId == id);
            if (rooms == null)
            {
                return NotFound();
            }

            return View(rooms);
        }

        // GET: Rooms/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomsId,RoomsName,RoomsDescription,Capacity,HourlyRate,City,IsAvailable")] Rooms rooms)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rooms);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rooms);
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rooms = await _context.Rooms.FindAsync(id);
            if (rooms == null)
            {
                return NotFound();
            }
            return View(rooms);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("RoomsId,RoomsName,RoomsDescription,Capacity,HourlyRate,City,IsAvailable")] Rooms rooms)
        {
            if (id != rooms.RoomsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rooms);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomsExists(rooms.RoomsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rooms);
        }

        // GET: Rooms/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rooms = await _context.Rooms
                .FirstOrDefaultAsync(m => m.RoomsId == id);
            if (rooms == null)
            {
                return NotFound();
            }

            return View(rooms);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rooms = await _context.Rooms.FindAsync(id);
            if (rooms != null)
            {
                _context.Rooms.Remove(rooms);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomsExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomsId == id);
        }
    }
}
