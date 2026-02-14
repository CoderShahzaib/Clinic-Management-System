
using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Core.DTOs
{
    public class DoctorRequestDTO
    {
        [Required(ErrorMessage = "Doctor name is required")]
        [StringLength(100, ErrorMessage = "Doctor name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Specialty is required")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters")]
        public string Specialization { get; set; } = string.Empty;

        public bool IsAvailable { get; set; }
    }
}
