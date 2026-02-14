
using ClinicManagementSystem.Core.Enums;

namespace ClinicManagementSystem.Core.DTOs
{
    public class AppointmentResponseDTO
    {
        public Guid Id { get; set; }
        public string PersonName { get; set; } = string.Empty;

        public string Date { get; set; }
        public string Time { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string Reason { get; set; } = string.Empty;

        public Guid? DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public string? DoctorNotes { get; set; }
    }
}
