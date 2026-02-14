using ClinicManagementSystem.Core.Domain.Entities;
using ClinicManagementSystem.Core.DTOs;

namespace ClinicManagementSystem.UI.ViewModel
{
    public class AdminDashboardViewModel
    {
        public List<AppointmentResponseDTO> Appointments { get; set; } = new();
        public List<DoctorResponseDTO> Doctors { get; set; } = new();

        public DoctorRequestDTO NewDoctor { get; set; } = new();
    }
}
