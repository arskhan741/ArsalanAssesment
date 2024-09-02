using System.ComponentModel.DataAnnotations;

namespace ArsalanAssesment.Web.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Roles { get; set; } = string.Empty;
    }
}
