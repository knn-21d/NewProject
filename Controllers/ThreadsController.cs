using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewProject.Data;
using NewProject.Models;

namespace NewProject.Controllers
{
    [Authorize]
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
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Threads.Include(t => t.User).Include(t => t.Answers);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Thread/Details/5
        [AllowAnonymous]
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
        public async Task<IActionResult> Create([Bind("Title,Text")] TopicStart topicStart)
        {

            if (topicStart.Title.Trim().Length == 0 || topicStart.Text.Trim().Length == 0 || !User.Identity.IsAuthenticated)
            {
                return View(topicStart);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            topicStart.ApplicationUserId = userId;
            topicStart.User = _context.ApplicationUsers.Where(usr => usr.Id == userId).First();
            _context.Add(topicStart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("ThreadId,Title,Text")] TopicStart topicStart)
        {
            if (id != topicStart.ThreadId)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (topicStart.Title.Trim().Length > 0 && topicStart.Text.Trim().Length > 0 && userId is not null)
            {
                try
                {
                    topicStart.ApplicationUserId = userId;
                    topicStart.User = _context.ApplicationUsers.Where(usr => usr.Id == userId).First();
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

        [HttpPost, ActionName("Answer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnswerToThread(int id, [Bind("Title,Text")] Answer answer)
        {
            if (_context.Threads == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Threads'  is null.");
            }
            var topicStart = await _context.Threads.FindAsync(id);
            if (topicStart == null || answer.Text.Trim().Length == 0)
            {
                return RedirectToAction(nameof(Details), id);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            answer.ApplicationUserId = userId;
            answer.Title = "";
            answer.User = _context.ApplicationUsers.Where(usr => usr.Id == userId).First();

            topicStart.Answers.Add(answer);
            _context.Update(topicStart);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new {id = id});

        }

        private bool TopicStartExists(int id)
        {
          return (_context.Threads?.Any(e => e.ThreadId == id)).GetValueOrDefault();
        }
    }
}
