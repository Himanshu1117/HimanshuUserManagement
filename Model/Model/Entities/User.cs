using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Model.Entities
{
    public class User
    {
        public int User_Id { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
        public string MiddleName { get; set; }

        [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters.")]

        public string LastName { get; set; }
       
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format")]
        [Required(ErrorMessage = "DateOfJoining is required")]
        public DateOnly DateOfJoining { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Invalid Date Format")]
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateOnly DOB { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email should be valid")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone]
        [StringLength(14, ErrorMessage = "Phone number cannot be longer than 14 characters.")]
        public string Phone { get; set; }

        [Phone]
        [StringLength(14, ErrorMessage = "Phone number cannot be longer than 14 characters.")]
        public string? AlternatePhone { get; set; }
        public string ImagePath { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created_at { get; set; } = DateTime.UtcNow;
        public DateTime Modified_at { get; set; } = DateTime.UtcNow;
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetExpiryToken { get; set; }

        // Navigation properties
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
