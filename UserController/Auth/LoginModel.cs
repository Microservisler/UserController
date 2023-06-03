using System.ComponentModel.DataAnnotations;

namespace UserController.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Kayıtlı Kullanıcı adı giriniz.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Geçerli Parola giriniz.")]
        public string? Password { get; set; }
    }
}
