using System.ComponentModel.DataAnnotations;

namespace AppSecurity_API.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Password name is required")]
        public string? Password { get; init; }
    }
}
