
namespace ClinicManagementSystem.Core.DTOs
{
    public class DoctorResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }
}
