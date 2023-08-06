using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewProject.Data;
using NewProject.Models;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace NewProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public IActionResult Index()
        {
            ViewData["ThreadsCount"] = _context.Threads.Count();
            ViewData["UsersCount"] = _context.ApplicationUsers.Count(); 
            ViewData["AllMessagesCount"] = _context.Answers.Count() + _context.Threads.Count();
            DateTime? lastAnswer = null;
            DateTime? lastThread = null;
            if (_context.Answers.Count() > 0)
            {
                lastAnswer = _context.Answers.OrderBy((i) => i.CreateDate).Last()?.CreateDate;
            }
            if (_context.Threads.Count() > 0)
            {
                lastThread = _context.Threads.OrderBy((i) => i.CreateDate).Last()?.CreateDate;
            }
            if (lastAnswer is not null && lastThread is not null)
            {
                ViewData["LastPostDate"] = lastAnswer > lastThread ? lastAnswer : lastThread;
            } else if (lastAnswer is not null)
            {
                ViewData["LastPostDate"] = lastAnswer;
            } else if (lastThread is not null)
            {
                ViewData["LastPostDate"] = lastThread;
            } else
            {
                ViewData["LastPostDate"] = "never";
            }
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost, ActionName("UploadPhoto")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file is null)
            {
                return View();
            } 
            byte[] bytes = default(byte[]);
            using (var memoryStream = new MemoryStream())
            {
                await file.OpenReadStream().CopyToAsync(memoryStream);
                bytes = memoryStream.ToArray();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.ApplicationUsers.Where(usr => usr.Id == userId).First();
            user.UserPic = bytes;
            _context.ApplicationUsers.Update(user);
            await _context.SaveChangesAsync();

            return Redirect("/Identity/Account/Manage");
            //return some action result e.g. return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}