using Data.Model.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Dto
{
     public class UserDetailsDto
    {


            public int? User_Id { get; set; }

            [Required(ErrorMessage = "FirstName is required")]
            [StringLength(30, ErrorMessage = "First name cannot be longer than 30 characters.")]
            public string FirstName { get; set; } = null!;


            [Required(ErrorMessage = "MiddleName is required")]
            [StringLength(30, ErrorMessage = "Middle name cannot be longer than 30 characters.")]
            public string MiddleName { get; set; } = null!;


            [StringLength(30, ErrorMessage = "Last name cannot be longer than 30 characters.")]
            public string? LastName { get; set; }



            [Required(ErrorMessage = "Gender is required")]
            public string Gender { get; set; }


            [DataType(DataType.Date, ErrorMessage = "Invalid Date Format")]
            [Required(ErrorMessage = "DateOfJoining is required")]
            public DateOnly DateOfJoining { get; set; }

            [Required(ErrorMessage = "DateOfBirth is required")]
            public DateOnly DOB { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Email should be valid")]
            [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phone is required")]
            [Phone]
            [StringLength(10, ErrorMessage = "Phone number cannot be longer than 10 characters.")]
            public string Phone { get; set; }

            [Phone]
            [StringLength(10, ErrorMessage = "Alternate phone number cannot be longer than 10 characters.")]
            public string? AlternatePhone { get; set; }

            public List<AddressDto>? Address { get; set; }
            public IFormFile? userImage { get; set; }
            public string? ImagePath { get; set; }
            public string? Password { get; set; }
            
            public string? AId { get; set; }
    }
}

  

