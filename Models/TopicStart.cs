using System.ComponentModel.DataAnnotations;

namespace NewProject.Models
{
    public class TopicStart : Post
    {
        [Key]
        public int ThreadId { get; set; }
        public List<Answer> Answers { get; set; }
        public int DisplayOrder { get; set; }
    }
}
