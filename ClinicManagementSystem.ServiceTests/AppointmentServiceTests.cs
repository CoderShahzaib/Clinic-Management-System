using ClinicManagementSystem.Core.Domain.Entities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Infrastructure.DatabaseContext;
using ClinicManagementSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagementSystem.ServiceTests
{
    public class AppointmentServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAppointmentAsync_ShouldAddAppointment_WhenValid()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            var request = new AppointmentRequestDTO
            {
                PersonName = "John",
                Date = DateOnly.FromDateTime(DateTime.Today).ToString(),
                Time = TimeOnly.FromDateTime(DateTime.Now).ToString(),
                Reason = "Checkup",
                PatientId = Guid.NewGuid()
            };

            var result = await service.AddAppointmentAsync(request);

            Assert.NotNull(result);
            Assert.Equal("John", result.PersonName);
            Assert.Equal(1, context.Appointments.Count());
        }

        [Fact]
        public async Task AddAppointmentAsync_ShouldThrow_WhenRequestIsNull()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.AddAppointmentAsync(null));
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldDelete_WhenExists()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            var patientId = Guid.NewGuid();

            var appointment = new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                patientId
            );

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var result = await service.DeleteAppointmentAsync(appointment.Id, patientId);

            Assert.True(result);
            Assert.Empty(context.Appointments);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_ShouldReturnFalse_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            var result = await service.DeleteAppointmentAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task AssignDoctorAsync_ShouldAssignDoctor_WhenAppointmentExists()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            var appointment = new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                Guid.NewGuid()
            );

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var doctorId = Guid.NewGuid();

            var result = await service.AssignDoctorAsync(appointment.Id, doctorId);

            Assert.True(result);

            var updated = await context.Appointments.FirstAsync();
            Assert.Equal(doctorId, updated.DoctorId);
        }

        [Fact]
        public async Task GetAppointmentsForPatientAsync_ShouldReturnOnlyPatientAppointments()
        {
            var context = GetDbContext();
            var service = new AppointmentService(context);

            var patientId = Guid.NewGuid();

            context.Appointments.Add(new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                patientId
            ));

            context.Appointments.Add(new Appointment(
                "Other",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Other",
                Guid.NewGuid()
            ));

            await context.SaveChangesAsync();

            var result = await service.GetAppointmentsForPatientAsync(patientId);

            Assert.Single(result);
            Assert.Equal("John", result.First().PersonName);
        }
    }
}
