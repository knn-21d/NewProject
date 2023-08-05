using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewProject.Models
{
    public class TopicStart : Post
    {
        [Key]
        public int ThreadId { get; set; }
        public List<Answer> Answers { get; set; } = new();
        public int DisplayOrder { get; set; }
        public DateTime CreateDate { get; }
    }
}
