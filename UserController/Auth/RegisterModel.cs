using System.ComponentModel.DataAnnotations;

namespace UserController.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Kullanıcı adını giriniz")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Lütfen Geçerli bir mail adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Lütfen Geçerli bir Parola giriniz.")]
        public string? Password { get; set; }
    }
}
