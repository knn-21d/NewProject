using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{
    public abstract class Post
    {
        [Required]
        [MaxLength(80)]
        public string Title { get; set; }
        public string Text { get; set; }
        //Relations
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }
        public DateTime CreateDate { get; }
    }
}
