﻿using System.ComponentModel.DataAnnotations;

namespace PaxQ1.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}
