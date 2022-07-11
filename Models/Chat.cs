using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GChat.Models
{
    public class Chat
    {
        public long ChatId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Members { get; set; }
        public Chat() => Members = new List<User>();

        public Message Message { get; set; }
    }
}