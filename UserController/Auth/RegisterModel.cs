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

        [Required(ErrorMessage = "Lütfen geçerli bir telefon numarası giriniz.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Lütfen isminiz giriniz.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lütfen soyisminizi giriniz.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Lütfen doğum tarihinizi giriniz.")]
        public DateTime BirthDate { get; set; }
    }
}
