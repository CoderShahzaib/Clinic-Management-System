using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(IAppointmentService appointmentService, IDoctorService doctorService, UserManager<ApplicationUser> userManager)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
            _userManager = userManager;
        }

        [HttpGet("Admin-Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var appointments = await _appointmentService.GetAllPatientsAsync();
            var doctors = await _doctorService.GetAllDoctorsAsync();

            var model = new AdminDashboardViewModel
            {
                Appointments = appointments.ToList(),
                Doctors = doctors.ToList()
            };

            return View(model);
        }

        [HttpPost("Assign-Doctor")]
        public async Task<IActionResult> AssignDoctor(Guid appointmentId, Guid doctorId)
        {
            await _appointmentService.AssignDoctorAsync(appointmentId, doctorId);
            return RedirectToAction("Dashboard");
        }

        [HttpPost("Remove-Doctor")]
        public async Task<IActionResult> RemoveDoctor(Guid doctorId)
        {
            await _doctorService.RemoveDoctorAsync(doctorId);
            return RedirectToAction("Dashboard");
        }

        [HttpPost("Add-Doctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDoctor(DoctorRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }

                return RedirectToAction("Dashboard");
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PersonName = model.Name
            };

            var password = GeneratePassword();

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return RedirectToAction("Dashboard");
            }

            await _userManager.AddToRoleAsync(user, "Doctor");
            await _doctorService.AddDoctorAsync(model, user.Id);

            TempData["DoctorPassword"] = password;

            return RedirectToAction("Dashboard");
        }





        [HttpPost("Remove-Appointment")]
        public async Task<IActionResult> RemoveAppointment(Guid appointmentId)
        {

            await _appointmentService.DeleteAppointmentAsAdminAsync(appointmentId);
            return RedirectToAction("Dashboard");
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(Guid doctorId, bool isAvailable)
        {
            var result = await _doctorService.SetAvailableAsync(doctorId, isAvailable);

            if (!result)
                return NotFound();

            return RedirectToAction("Dashboard");
        }

        private string GeneratePassword()
        {
            return $"Doc@{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }

    }

}
