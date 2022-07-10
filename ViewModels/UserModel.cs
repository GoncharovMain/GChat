#nullable disable

namespace GChat.ViewModels
{
    public class UserModel
    {
        public long Id { get; set; }
        public string Login { get; set; }

        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Sex { get; set; }
        public DateTime Birthday { get; set; }

    }
}