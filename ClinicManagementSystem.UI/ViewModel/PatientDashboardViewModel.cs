using ClinicManagementSystem.Core.DTOs;

namespace ClinicManagementSystem.UI.ViewModel
{
    public class PatientDashboardViewModel
    {
        public List<AppointmentResponseDTO> Appointments { get; set; } = new();
        public AppointmentRequestDTO NewAppointment { get; set; } = new();


    }
}
