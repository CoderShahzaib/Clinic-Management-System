using ClinicManagementSystem.Core.Domain.IdentityEntities;

namespace ClinicManagementSystem.Core.Domain.Entities
{
    public class Doctor
    {
        public Guid Id { get; private set; }

        public Guid UserId { get; private set; } 
        public ApplicationUser User { get; private set; }

        public string Name { get; private set; }
        public string Specialization { get; private set; }
        public bool IsAvailable { get; set; }

        private Doctor() { }

        public Doctor(Guid userId, string name, string specialization, bool isAvailable)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Specialization = specialization;
            IsAvailable = isAvailable;
        }

        public void SetAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }

}
