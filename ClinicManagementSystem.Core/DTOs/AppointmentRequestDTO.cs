using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Core.DTOs
{
    public class AppointmentRequestDTO
    {
        public Guid Id { get; set; }

        [Required]
        public string PersonName { get; set; } = string.Empty;

        [Required]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public Guid DoctorId { get; set; }

        public Guid PatientId { get; set; }

        public List<DoctorResponseDTO> AvailableDoctors { get; set; } = new();
    }

}
