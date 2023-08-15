using Microsoft.AspNetCore.Mvc;
using NewProject.Data;
using System.Security.Claims;

namespace NewProject.Controllers
{
    public class AppUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppUserController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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
        }

    }
}
