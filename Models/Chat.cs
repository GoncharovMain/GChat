using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GChat.Models
{
    public class Chat
    {
        public long ChatId { get; set; }
        public string Name { get; set; }
        public List<User> Members { get; set; }

        public Message Message { get; set; }
    }
}