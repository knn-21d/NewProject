using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewProject.Data;
using NewProject.Models;

namespace NewProject.Controllers
{
    public class ThreadsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ThreadsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Thread
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Threads.Include(t => t.User).Include(t => t.Answers);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Thread/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Threads == null)
            {
                return NotFound();
            }

            var topicStart = await _context.Threads
                .Include(t => t.User)
                .Include(t => t.Answers)
                .FirstOrDefaultAsync(m => m.ThreadId == id);
            if (topicStart == null)
            {
                return NotFound();
            }

            return View(topicStart);
        }

        // GET: Thread/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thread/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ThreadId,Title,Text,ApplicationUserId")] TopicStart topicStart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(topicStart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(topicStart);
        }

        // GET: Thread/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Threads == null)
            {
                return NotFound();
            }

            var topicStart = await _context.Threads.FindAsync(id);
            if (topicStart == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", topicStart.ApplicationUserId);
            return View(topicStart);
        }

        // POST: Thread/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ThreadId,Title,Text,ApplicationUserId")] TopicStart topicStart)
        {
            if (id != topicStart.ThreadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topicStart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TopicStartExists(topicStart.ThreadId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", topicStart.ApplicationUserId);
            return View(topicStart);
        }

        // GET: Thread/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Threads == null)
            {
                return NotFound();
            }

            var topicStart = await _context.Threads
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.ThreadId == id);
            if (topicStart == null)
            {
                return NotFound();
            }

            return View(topicStart);
        }

        // POST: Thread/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Threads == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Threads'  is null.");
            }
            var topicStart = await _context.Threads.FindAsync(id);
            if (topicStart != null)
            {
                _context.Threads.Remove(topicStart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TopicStartExists(int id)
        {
          return (_context.Threads?.Any(e => e.ThreadId == id)).GetValueOrDefault();
        }
    }
}
