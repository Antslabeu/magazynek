namespace Magazynek.Entities.Models
{
    using System.ComponentModel.DataAnnotations;

    class LoginModel
    {
        [Required(ErrorMessage = "Wpisz e-mail")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy e-mail")]
        public string? mail { get; set; }

        [Required(ErrorMessage = "Wpisz hasło")]
        [MinLength(6, ErrorMessage = "Min. 6 znaków")]
        public string? password { get; set; }
        public bool IsEmpty { get => mail == null || password == null; }
        public void Clear()
        {
            mail = string.Empty;
            password = string.Empty;
        }
    }
    class RevertPasswordModel
    {
        [Required(ErrorMessage = "Wpisz e-mail")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy e-mail")]
        public string? mail { get; set; }

        public string? password { get; set; }
        public bool IsEmpty { get => mail == null || password == null; }
        public void Clear()
        {
            mail = string.Empty;
            password = string.Empty;
        }
    }
}