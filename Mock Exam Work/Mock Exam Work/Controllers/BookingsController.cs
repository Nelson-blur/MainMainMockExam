using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mock_Exam_Work.Data;
using Mock_Exam_Work.Models;

namespace Mock_Exam_Work.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
           
            var userBookingsData = User.IsInRole("Admin")
                ? await _context.Bookings.Include(b => b.Room).ToListAsync()  // Admin sees all
                : await _context.Bookings.Where(b => b.UserId == userId).Include(b => b.Room).ToListAsync();  // Users see only theirs

            return View(userBookingsData);
        }
        
        
        

        

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["RoomsId"] = new SelectList(_context.Set<Rooms>(), "RoomsId", "RoomsId");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingsId,UserId,RoomsId,CheckInDate,CheckOutDate,Status,BookingCreatedAt,SpecialRequest,IsPayed,PayedAt")] Bookings bookings)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null)
            {
                return NotFound();
            }
            bookings.UserId = UserId;
            ModelState.Remove("UserId");

            bookings.UserId = UserId;
            ModelState.Remove("UserId");
            ModelState.Remove("Room");

            if (ModelState.IsValid)
            {
                _context.Add(bookings);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomsId"] = new SelectList(_context.Set<Rooms>(), "RoomsId", "RoomsId", bookings.RoomsId);
            return View(bookings);
        }

        // GET: Bookings/Edit/5
        

        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings == null)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (bookings.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid(); // User doesn't own this booking and isn't admin
            }
            ViewData["RoomsId"] = new SelectList(_context.Set<Rooms>(), "RoomsId", "RoomsId", bookings.RoomsId);
            return View(bookings);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingsId,UserId,RoomsId,CheckInDate,CheckOutDate,Status,BookingCreatedAt,SpecialRequest,IsPayed,PayedAt")] Bookings bookings)
        {
            if (id != bookings.BookingsId)
            {
                return NotFound();
            }
            // Check if user owns this booking or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (bookings.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookings);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingsExists(bookings.BookingsId))
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
            ViewData["RoomsId"] = new SelectList(_context.Set<Rooms>(), "RoomsId", "RoomsId", bookings.RoomsId);
            return View(bookings);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (bookings == null)
            {
                return NotFound();
            }
            // Check if user owns this booking or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (bookings.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(bookings);
        }

       

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookings = await _context.Bookings.FindAsync(id);
            if (bookings != null)
            {
               return NotFound();
            }
            // Check if user owns this booking or is admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (bookings.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Bookings.Remove(bookings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingsExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingsId == id);
        }
    }
}
