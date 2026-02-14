using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ClinicManagementSystem.ControllerTests
{
    public class AccountControllerTest
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<ApplicationRole>> _roleManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IEmailService> _emailServiceMock;

        private readonly AccountController _controller;

        public AccountControllerTest()
        {
            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null
            );

            var roleStore = new Mock<IRoleStore<ApplicationRole>>();
            _roleManagerMock = new Mock<RoleManager<ApplicationRole>>(
                roleStore.Object, null, null, null, null
            );

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object,
                new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                null, null, null, null
            );

            _emailServiceMock = new Mock<IEmailService>();

            _controller = new AccountController(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _signInManagerMock.Object,
                _emailServiceMock.Object
            );
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            var result = _controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_Get_ReturnsView()
        {
            var result = _controller.Register();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_Post_ValidModel_RedirectsToLogin()
        {
            var dto = new RegisterDTO
            {
                UserName = "Test User",
                Email = "test@gmail.com",
                Password = "Password123!",
                Role = "Patient"
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _roleManagerMock
                .Setup(x => x.RoleExistsAsync(dto.Role))
                .ReturnsAsync(true);

            var result = await _controller.Register(dto);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }

        [Fact]
        public async Task Login_Post_UserNotFound_ReturnsView()
        {
            var dto = new LoginDTO
            {
                Email = "notfound@gmail.com",
                Password = "123456"
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _controller.Login(dto);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ForgotPassword_UserNotFound_ReturnsView()
        {
            var dto = new ForgotPasswordDTO
            {
                Email = "unknown@gmail.com"
            };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _controller.ForgotPassword(dto);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            var result = await _controller.Logout();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
        }
    }
}
