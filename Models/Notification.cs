using System.ComponentModel.DataAnnotations;

namespace BanPhimCanhCach.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Url { get; set; }
        public bool Viewed { get; set; } = false;
        public DateTime CreatedDate { get; set; }
    }
}
