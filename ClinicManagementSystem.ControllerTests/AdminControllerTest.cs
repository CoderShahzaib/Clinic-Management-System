using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.Controllers;
using ClinicManagementSystem.UI.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagementSystem.ControllerTests
{
    public class AdminControllerTests
    {
        private readonly Mock<IAppointmentService> _appointmentServiceMock;
        private readonly Mock<IDoctorService> _doctorServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _appointmentServiceMock = new Mock<IAppointmentService>();
            _doctorServiceMock = new Mock<IDoctorService>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStore.Object,
                null, null, null, null, null, null, null, null
            );

            _controller = new AdminController(
                _appointmentServiceMock.Object,
                _doctorServiceMock.Object,
                _userManagerMock.Object
            );

            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new Mock<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
        }

        [Fact]
        public async Task Dashboard_ReturnsViewWithModel()
        {
            _appointmentServiceMock
                .Setup(x => x.GetAllPatientsAsync())
                .ReturnsAsync(new List<AppointmentResponseDTO>());

            _doctorServiceMock
                .Setup(x => x.GetAllDoctorsAsync())
                .ReturnsAsync(new List<DoctorResponseDTO>());

            var result = await _controller.Dashboard();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<AdminDashboardViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task AssignDoctor_RedirectsToDashboard()
        {
            var appointmentId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();

            var result = await _controller.AssignDoctor(appointmentId, doctorId);

            _appointmentServiceMock.Verify(
                x => x.AssignDoctorAsync(appointmentId, doctorId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task RemoveDoctor_RedirectsToDashboard()
        {
            var doctorId = Guid.NewGuid();

            var result = await _controller.RemoveDoctor(doctorId);

            _doctorServiceMock.Verify(
                x => x.RemoveDoctorAsync(doctorId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task AddDoctor_InvalidModel_RedirectsToDashboard()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var dto = new DoctorRequestDTO();

            var result = await _controller.AddDoctor(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task AddDoctor_CreateFails_RedirectsToDashboard()
        {
            var dto = new DoctorRequestDTO
            {
                Name = "Dr Test",
                Email = "dr@test.com"
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await _controller.AddDoctor(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task AddDoctor_Success_RedirectsToDashboard()
        {
            var dto = new DoctorRequestDTO
            {
                Name = "Dr Test",
                Email = "dr@test.com"
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock
                .Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Doctor"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _controller.AddDoctor(dto);

            _doctorServiceMock.Verify(
                x => x.AddDoctorAsync(dto, It.IsAny<Guid>()),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);

            Assert.True(_controller.TempData.ContainsKey("DoctorPassword"));
        }

        // ✅ RemoveAppointment Test
        [Fact]
        public async Task RemoveAppointment_RedirectsToDashboard()
        {
            var appointmentId = Guid.NewGuid();

            var result = await _controller.RemoveAppointment(appointmentId);

            _appointmentServiceMock.Verify(
                x => x.DeleteAppointmentAsAdminAsync(appointmentId),
                Times.Once
            );

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_Success_RedirectsToDashboard()
        {
            var doctorId = Guid.NewGuid();

            _doctorServiceMock
                .Setup(x => x.SetAvailableAsync(doctorId, true))
                .ReturnsAsync(true);

            var result = await _controller.Edit(doctorId, true);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenServiceReturnsFalse()
        {
            var doctorId = Guid.NewGuid();

            _doctorServiceMock
                .Setup(x => x.SetAvailableAsync(doctorId, false))
                .ReturnsAsync(false);

            var result = await _controller.Edit(doctorId, false);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
