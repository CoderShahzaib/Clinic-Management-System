using ClinicManagementSystem.Core.DTOs;

namespace ClinicManagementSystem.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentResponseDTO>> GetAllPatientsAsync();

        Task<AppointmentResponseDTO> AddAppointmentAsync(AppointmentRequestDTO request);

        Task<bool> DeleteAppointmentAsync(Guid appointmentId, Guid userId);

        Task<bool> DeleteAppointmentAsAdminAsync(Guid appointmentId);
        Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsForDoctorAsync(Guid userId);

        Task<bool> AssignDoctorAsync(Guid appointmentId, Guid doctorId);

        Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsForPatientAsync(Guid userId);
    }
}
