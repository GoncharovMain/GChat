using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace GChat.Models
{
    public class User
    {
        public long UserId { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Sex { get; set; }
        public long Birthday { get; set; }

        public Chat Chat { get; set; }
        public Message Message { get; set; }


        //public List<User> Friends { get; set; }
        //public User ThisUser { get; set; }
    }
}