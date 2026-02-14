using ClinicManagementSystem.Core.Interfaces;
using ClinicManagementSystem.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClinicManagementSystem.UI.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IDoctorService _doctorService;
        public DoctorController(IAppointmentService appointmentService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _doctorService = doctorService;
        }

        [HttpGet("Doctor-Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);

            if (doctorId == null)
                return Unauthorized();

            var appointments = await _appointmentService.GetAppointmentsForDoctorAsync(doctorId.Value);

            var vm = new DoctorDashboardViewModel
            {
                Appointments = appointments
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteAppointment(Guid appointmentId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);

            if (doctorId == null)
                return Unauthorized();

            await _doctorService.MarkAsCompletedAsync(doctorId.Value, appointmentId);
            return RedirectToAction(nameof(Dashboard));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotes(Guid appointmentId, string notes)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);

            if (doctorId == null)
                return Unauthorized();

            await _doctorService.AddNotesAsync(doctorId.Value, appointmentId, notes);
            return RedirectToAction(nameof(Dashboard));
        }

    }
}
