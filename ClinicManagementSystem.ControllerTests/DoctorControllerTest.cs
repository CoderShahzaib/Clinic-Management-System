using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.Controllers;
using ClinicManagementSystem.UI.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagementSystem.ControllerTests
{
    public class DoctorControllerTests
    {
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly DoctorController _controller;

        public DoctorControllerTests()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _doctorServiceMock = new Mock<IDoctorService>();

            _controller = new DoctorController(
                _appointmentServiceMock.Object,
                _doctorServiceMock.Object
            );
        }

        private void SetUser(Guid userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = user
                }
            };
        }

        [Fact]
        public async Task Dashboard_ReturnsUnauthorized_WhenDoctorIdNull()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync((Guid?)null);

            var result = await _controller.Dashboard();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Dashboard_ReturnsViewWithModel()
        {
            var userId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();

            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync(doctorId);

            _appointmentServiceMock
                .Setup(x => x.GetAppointmentsForDoctorAsync(doctorId))
                .ReturnsAsync(new List<AppointmentResponseDTO>());

            var result = await _controller.Dashboard();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<DoctorDashboardViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task CompleteAppointment_ReturnsUnauthorized_WhenDoctorIdNull()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync((Guid?)null);

            var result = await _controller.CompleteAppointment(Guid.NewGuid());

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CompleteAppointment_RedirectsToDashboard()
        {
            var userId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();

            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync(doctorId);

            var result = await _controller.CompleteAppointment(appointmentId);

            _doctorServiceMock.Verify(
                x => x.MarkAsCompletedAsync(doctorId, appointmentId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task AddNotes_ReturnsUnauthorized_WhenDoctorIdNull()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync((Guid?)null);

            var result = await _controller.AddNotes(Guid.NewGuid(), "Test Notes");

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AddNotes_RedirectsToDashboard()
        {
            var userId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();

            SetUser(userId);

            _doctorServiceMock
                .Setup(x => x.GetDoctorIdByUserIdAsync(userId))
                .ReturnsAsync(doctorId);

            var result = await _controller.AddNotes(appointmentId, "Patient is stable");

            _doctorServiceMock.Verify(
                x => x.AddNotesAsync(doctorId, appointmentId, "Patient is stable"),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }
    }
}
