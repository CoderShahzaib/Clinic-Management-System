using ClinicManagementSystem.Core.Domain.Entities;
using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Enums;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinicManagementSystem.Infrastructure.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DoctorService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AddNotesAsync(Guid doctorId, Guid appointmentId, string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return false;

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

            if (appointment == null)
                return false; 

            appointment.DoctorNotes = notes;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(Guid doctorId, Guid appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.DoctorId == doctorId);

            if (appointment == null || appointment.Status != AppointmentStatus.Pending)
                return false;

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorResponseDTO>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.Select(d => new DoctorResponseDTO
            {
                Id = d.Id,
                Name = d.Name,
                Specialization = d.Specialization,
                IsAvailable = d.IsAvailable
            }).ToListAsync();
        }

        public async Task<bool> SetAvailableAsync(Guid doctorId, bool isAvailable)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;

            doctor.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveDoctorAsync(Guid doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return false;

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            _context.Appointments.RemoveRange(appointments);

            _context.Doctors.Remove(doctor);

            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(doctor.UserId.ToString());
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return true;
        }


        public async Task<bool> AddDoctorAsync(DoctorRequestDTO dto, Guid userId)
        {
            var doctor = new Doctor(
                userId,
                dto.Name,
                dto.Specialization,
                dto.IsAvailable
            );

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<IEnumerable<AppointmentResponseDTO>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Select(a => new AppointmentResponseDTO
                {
                    Id = a.Id,
                    PersonName = a.PersonName,
                    Date = a.Date,
                    Time = a.Time,
                    Reason = a.Reason,
                    Status = a.Status,
                    DoctorNotes = a.DoctorNotes
                })
                .ToListAsync();

            return appointments;
        }

        public async Task<Guid?> GetDoctorIdByUserIdAsync(Guid userId)
        {
            return await _context.Doctors
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefaultAsync();
        }

    }
}
