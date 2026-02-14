using ClinicManagementSystem.Core.DTOs;
using System.Collections.Generic;

namespace ClinicManagementSystem.UI.ViewModels
{
    public class DoctorDashboardViewModel
    {
        public IEnumerable<AppointmentResponseDTO> Appointments { get; set; }
            = new List<AppointmentResponseDTO>();
    }
}
