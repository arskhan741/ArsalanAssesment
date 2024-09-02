using System.ComponentModel.DataAnnotations;

namespace ArsalanAssesment.Web.DTOs.UserDTOs
{
    public class LoginUserDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
