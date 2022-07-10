using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace GChat.Models
{
    [Keyless]
    public class Message
    {
        public long MessageId { get; set; }

        public long UserForeignKey { get; set; }
        public User User { get; set; }

        public long ChatForeignKey { get; set; }
        public Chat Chat { get; set; }

        public string Text { get; set; }
        public long PublishedDate { get; set; }

    }
}