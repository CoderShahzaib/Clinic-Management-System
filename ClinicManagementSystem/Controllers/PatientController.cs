using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagementSystem.UI.Controllers
{
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;

        public PatientController(IAppointmentService appointmentService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        [HttpGet("Patient-Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var appointments = await _appointmentService.GetAppointmentsForPatientAsync(userId);
            var doctors = await _doctorService.GetAllDoctorsAsync();

            var model = new PatientDashboardViewModel
            {
                Appointments = appointments.ToList(),
                NewAppointment = new AppointmentRequestDTO
                {
                    AvailableDoctors = doctors.ToList()
                }
            };

            return View(model);
        }

        [HttpPost("Book-Appointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment([Bind(Prefix = "NewAppointment")] AppointmentRequestDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.AvailableDoctors = (await _doctorService.GetAllDoctorsAsync()).ToList();
                var model = new PatientDashboardViewModel
                {
                    Appointments = new List<AppointmentResponseDTO>(),
                    NewAppointment = dto
                };
                return View("Dashboard", model);
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            dto.PatientId = userId;

            await _appointmentService.AddAppointmentAsync(dto);
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost("Cancel-Appointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _appointmentService.DeleteAppointmentAsync(appointmentId, userId);

            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost("Delete-Appointment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(Guid appointmentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _appointmentService.DeleteAppointmentAsync(appointmentId, userId);
            return RedirectToAction(nameof(Dashboard));
        }


    }
}
