using System.ComponentModel.DataAnnotations;

namespace AppSecurity_API.Dtos
{
    public class CreateUserDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
