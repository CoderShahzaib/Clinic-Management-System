using ClinicManagementSystem.Core.Enums;

namespace ClinicManagementSystem.Core.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; }
        public string PersonName { get; private set; }
        public string Date { get; private set; }
        public string Time { get; private set; }
        public string Reason { get; private set; }

        public Guid? DoctorId { get; private set; }

        public Guid PatientId { get; private set; }
        public Doctor? Doctor { get; private set; }

        public string? DoctorNotes { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public Appointment(
            string personName,
            string date,
            string time,
            string reason,
            Guid patientId
        )
        {
            Id = Guid.NewGuid();
            PersonName = personName;
            Date = date;
            Time = time;
            Reason = reason;
            PatientId = patientId;
        }


        public void AssignDoctor(Guid doctorId)
        {
            DoctorId = doctorId;
        }

        public void AddDoctorNotes(Guid doctorId, string notes)
        {
            if (DoctorId != doctorId)
                throw new InvalidOperationException("Only assigned doctor can add notes");

            if (string.IsNullOrWhiteSpace(notes))
                throw new ArgumentException("Doctor notes are required");

            DoctorNotes = notes;
        }
    }

}
