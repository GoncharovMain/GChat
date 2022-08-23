using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace GChat.Models
{
    public class User
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity), Key()]
        public long UserId { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Sex { get; set; }
        public long Birthday { get; set; }

        public virtual ICollection<Chat> Chats { get; set; }
        public Message Message { get; set; }

        public User() => Chats = new List<Chat>();


        //public List<User> Friends { get; set; }
        //public User ThisUser { get; set; }
    }
}