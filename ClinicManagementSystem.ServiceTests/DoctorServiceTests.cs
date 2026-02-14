using ClinicManagementSystem.Core.Domain.Entities;
using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Enums;
using ClinicManagementSystem.Infrastructure.DatabaseContext;
using ClinicManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagementSystem.ServiceTests
{
    public class DoctorServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();

            return new Mock<UserManager<ApplicationUser>>(
                store.Object,
                null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task AddDoctorAsync_ShouldAddDoctor()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var service = new DoctorService(context, userManager.Object);

            var dto = new DoctorRequestDTO
            {
                Name = "Dr John",
                Specialization = "Cardiology",
                IsAvailable = true
            };

            var userId = Guid.NewGuid();

            var result = await service.AddDoctorAsync(dto, userId);

            Assert.True(result);
            Assert.Equal(1, context.Doctors.Count());
        }

        [Fact]
        public async Task SetAvailableAsync_ShouldUpdateAvailability()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var doctor = new Doctor(Guid.NewGuid(), "Dr A", "Neuro", true);
            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context, userManager.Object);

            var result = await service.SetAvailableAsync(doctor.Id, false);

            Assert.True(result);
            Assert.False(context.Doctors.First().IsAvailable);
        }

        [Fact]
        public async Task AddNotesAsync_ShouldAddNotes_WhenValid()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var doctorId = Guid.NewGuid();

            var appointment = new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                Guid.NewGuid()
            );

            appointment.AssignDoctor(doctorId);

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var service = new DoctorService(context, userManager.Object);

            var result = await service.AddNotesAsync(doctorId, appointment.Id, "Patient stable");

            Assert.True(result);
            Assert.Equal("Patient stable", context.Appointments.First().DoctorNotes);
        }

        [Fact]
        public async Task MarkAsCompletedAsync_ShouldUpdateStatus_WhenPending()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var doctorId = Guid.NewGuid();

            var appointment = new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                Guid.NewGuid()
            );

            appointment.AssignDoctor(doctorId);

            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var service = new DoctorService(context, userManager.Object);

            var result = await service.MarkAsCompletedAsync(doctorId, appointment.Id);

            Assert.True(result);
            Assert.Equal(AppointmentStatus.Completed, context.Appointments.First().Status);
        }

        [Fact]
        public async Task RemoveDoctorAsync_ShouldDeleteDoctorAndAppointments()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var doctorUserId = Guid.NewGuid();
            var doctor = new Doctor(doctorUserId, "Dr Remove", "Skin", true);

            context.Doctors.Add(doctor);

            var appointment = new Appointment(
                "John",
                DateOnly.FromDateTime(DateTime.Today).ToString(),
                TimeOnly.FromDateTime(DateTime.Now).ToString(),
                "Checkup",
                Guid.NewGuid()
            );

            appointment.AssignDoctor(doctor.Id);
            context.Appointments.Add(appointment);

            await context.SaveChangesAsync();

            userManager
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            userManager
                .Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(IdentityResult.Success);

            var service = new DoctorService(context, userManager.Object);

            var result = await service.RemoveDoctorAsync(doctor.Id);

            Assert.True(result);
            Assert.Empty(context.Doctors);
            Assert.Empty(context.Appointments);
        }

        [Fact]
        public async Task GetDoctorIdByUserIdAsync_ShouldReturnDoctorId()
        {
            var context = GetDbContext();
            var userManager = GetUserManagerMock();

            var userId = Guid.NewGuid();
            var doctor = new Doctor(userId, "Dr A", "Cardio", true);

            context.Doctors.Add(doctor);
            await context.SaveChangesAsync();

            var service = new DoctorService(context, userManager.Object);

            var result = await service.GetDoctorIdByUserIdAsync(userId);

            Assert.Equal(doctor.Id, result);
        }
    }
}
