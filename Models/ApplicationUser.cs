using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NewProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public byte[]? UserPic { get; set; }
        [Required]
        public bool IsAdmin { get; set; } = false;
    }
}