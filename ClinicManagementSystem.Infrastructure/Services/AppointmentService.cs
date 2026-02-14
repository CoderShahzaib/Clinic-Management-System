using ClinicManagementSystem.Core.Domain.Entities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;


namespace ClinicManagementSystem.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {

        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentResponseDTO> AddAppointmentAsync(AppointmentRequestDTO request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.DoctorId != Guid.Empty)
            {
                var doctorExists = await _context.Doctors
                    .AnyAsync(d => d.Id == request.DoctorId);

                if (!doctorExists)
                    throw new InvalidOperationException("Selected doctor does not exist");
            }

            var appointment = new Appointment(
                personName: request.PersonName,
                date: request.Date,
                time: request.Time,
                reason: request.Reason,
                patientId: request.PatientId
            );

            if (request.DoctorId != Guid.Empty)
            {
                appointment.AssignDoctor(request.DoctorId);
            }

            _context.Appointments.Add(appointment);

            await _context.SaveChangesAsync();

            return new AppointmentResponseDTO
            {
                Id = appointment.Id,
                Date = appointment.Date,
                Time = appointment.Time,
                PersonName = appointment.PersonName,
                Reason = appointment.Reason
            };
        }

        public async Task<bool> AssignDoctorAsync(Guid appointmentId, Guid doctorId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
                return false;

            appointment.AssignDoctor(doctorId);

            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<bool> DeleteAppointmentAsync(Guid appointmentId, Guid userId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == userId);

            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }


        public async Task<bool> DeleteAppointmentAsAdminAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false; 

            _context.Appointments.Remove(appointment);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }



        public async Task<IEnumerable<AppointmentResponseDTO>> GetAllPatientsAsync()
        {
            var appointments = await (
                from a in _context.Appointments
                join d in _context.Doctors
                    on a.DoctorId equals d.Id into doctorGroup
                from d in doctorGroup.DefaultIfEmpty() 
                select new AppointmentResponseDTO
                {
                    Id = a.Id,
                    Date = a.Date,
                    Time = a.Time,
                    PersonName = a.PersonName,
                    Reason = a.Reason,
                    Status = a.Status,
                    DoctorId = a.DoctorId,
                    DoctorName = d != null ? d.Name : null
                }
            ).ToListAsync();

            return appointments;
        }



        public async Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsForDoctorAsync(Guid userId)
        {

            if (userId == Guid.Empty)
                return Enumerable.Empty<AppointmentResponseDTO>();

            var appointments = await _context.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == userId)
                .Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    Date = a.Date,
                    Time = a.Time,
                    PersonName = a.PersonName,
                    Reason = a.Reason,
                    Status = a.Status
                })
                .ToListAsync();

            return appointments;
        }

        public async Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsForPatientAsync(Guid userId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Where(a => a.PatientId == userId)
                .Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    Date = a.Date,
                    Time = a.Time,
                    PersonName = a.PersonName,
                    Reason = a.Reason,
                    Status = a.Status
                })
                .ToListAsync();
        }


    }
}
