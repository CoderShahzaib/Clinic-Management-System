using ClinicManagementSystem.Core.DTOs;


namespace ClinicManagementSystem.Core.Interfaces
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorResponseDTO>> GetAllDoctorsAsync();

        Task<bool> AddDoctorAsync(DoctorRequestDTO request, Guid userId);
        Task<bool> MarkAsCompletedAsync(Guid doctorId, Guid appointmentId);

        Task<bool> AddNotesAsync(Guid doctorId, Guid appointmentId, string notes);

        Task<bool> SetAvailableAsync(Guid doctorId, bool isAvailable);

        Task<bool> RemoveDoctorAsync(Guid appointmentId);

        Task<Guid?> GetDoctorIdByUserIdAsync(Guid userId);

    }
}
