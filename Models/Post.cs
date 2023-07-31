using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{
    public abstract class Post
    {
        [Required]
        public string Title { get; set; }
        public string Text { get; set; }
        //Relations
        public string IdentityUserId { get; set; }
        [ForeignKey("IdentityUserId")]
        public ApplicationUser User { get; set; }
    }
}
