using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.Controllers;
using ClinicManagementSystem.UI.ViewModel;
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
    public class PatientControllerTests
    {
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly PatientController _controller;

        public PatientControllerTests()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _doctorServiceMock = new Mock<IDoctorService>();

            _controller = new PatientController(
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
        public async Task Dashboard_ReturnsViewWithModel()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _appointmentServiceMock
                .Setup(x => x.GetAppointmentsForPatientAsync(userId))
                .ReturnsAsync(new List<AppointmentResponseDTO>());

            _doctorServiceMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(new List<DoctorResponseDTO>());

            var result = await _controller.Dashboard();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<PatientDashboardViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task BookAppointment_InvalidModel_ReturnsDashboardView()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            _controller.ModelState.AddModelError("DoctorId", "Required");

            _doctorServiceMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(new List<DoctorResponseDTO>());

            var dto = new AppointmentRequestDTO();

            var result = await _controller.BookAppointment(dto);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Dashboard", viewResult.ViewName);
        }

        [Fact]
        public async Task BookAppointment_Success_RedirectsToDashboard()
        {
            var userId = Guid.NewGuid();
            SetUser(userId);

            var dto = new AppointmentRequestDTO();

            var result = await _controller.BookAppointment(dto);

            _appointmentServiceMock.Verify(
                x => x.AddAppointmentAsync(It.IsAny<AppointmentRequestDTO>()),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task CancelAppointment_RedirectsToDashboard()
        {
            var userId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();
            SetUser(userId);

            var result = await _controller.CancelAppointment(appointmentId);

            _appointmentServiceMock.Verify(
                x => x.DeleteAppointmentAsync(appointmentId, userId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task DeleteAppointment_RedirectsToDashboard()
        {
            var userId = Guid.NewGuid();
            var appointmentId = Guid.NewGuid();
            SetUser(userId);

            var result = await _controller.DeleteAppointment(appointmentId);

            _appointmentServiceMock.Verify(
                x => x.DeleteAppointmentAsync(appointmentId, userId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }
    }
}
