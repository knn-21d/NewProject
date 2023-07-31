using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{
    public class Answer : Post
    {
        [Key]
        public int AnswerId { get; set; }
        //Relations
        [Required]
        public TopicStart Thread { get; set; }
        [ForeignKey("ThreadId")]
        public int ThreadId { get; set; }
    }
}
