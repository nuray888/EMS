using System.ComponentModel.DataAnnotations;

namespace UnitedPayment.Model
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; }

    }
}
