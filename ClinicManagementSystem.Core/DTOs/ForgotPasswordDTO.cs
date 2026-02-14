using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClinicManagementSystem.Core.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
