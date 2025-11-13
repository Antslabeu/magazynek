using Humanizer;

namespace Magazynek.Entities.Models
{
    using System.ComponentModel.DataAnnotations;
    

    public class NewAccountModel
    {
        [Required(ErrorMessage = "Podaj imię")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Imię i nazwisko: 2–80 znaków")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Wpisz e-mail")]
        [StringLength(80, MinimumLength = 2, ErrorMessage = "Mail: 2–80 znaków")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy e-mail")]
        [Display(Name = "E-mail (login)")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Wpisz hasło")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło: 6–100 znaków")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool IsEmpty
        {
            get =>
                Name == null ||
                Login == null ||
                Password == null;
        }
        public void Clear()
{
    Name = string.Empty;
    Login = string.Empty;
    Password = string.Empty;
}

        
    }
}