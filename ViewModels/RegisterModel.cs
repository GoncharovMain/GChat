using System.ComponentModel.DataAnnotations;

#nullable disable

namespace GChat.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Login.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Укажите ваше имя.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Укажите вашу фамилию.")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Укажите дату рождения.")]
        [DataType(DataType.DateTime)]
        public DateTime Birthday { get; set; }


        [Required(ErrorMessage = "Укажите свой пол.")]
        public string Sex { get; set; }



    }
}