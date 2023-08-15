using Microsoft.AspNetCore.Mvc;
using NewProject.Data;
using NewProject.Models;
using System.Security.Claims;

namespace NewProject.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ActionName("Answer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnswerToThread(int id, string text)
        {
            if (_context.Threads == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Threads'  is null.");
            }
            var topicStart = await _context.Threads.FindAsync(id);
            if (topicStart == null || text.Trim().Length == 0)
            {
                return RedirectToAction("Details", "Threads", id);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var answer = new Answer
            {
                ApplicationUserId = userId,
                Title = "",
                User = _context.ApplicationUsers.Where(usr => usr.Id == userId).First(),
                Text = text
            };

            topicStart.Answers.Add(answer);
            _context.Update(topicStart);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Threads", new { id = id });
        }

        [HttpPost, ActionName("DeleteAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAnswer(int answerId)
        {
            if (_context.Threads == null || _context.Answers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Threads'  is null.");
            }
            var answer = await _context.Answers.FindAsync(answerId);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Threads", new { id = answer.ThreadId });
            }

            return Problem("Not found entity by id");
        }

        [HttpPost, ActionName("EditAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAnswer(int answerId, string newText)
        {
            if (_context.Threads == null || _context.Answers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Threads'  is null.");
            }
            var answer = await _context.Answers.FindAsync(answerId);
            if (answer != null && newText.Trim().Length > 0)
            {
                answer.Text = newText;
                _context.Answers.Update(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Threads", new { id = answer.ThreadId });
            }

            return Problem("Not found entity by id");
        }
    }
}
