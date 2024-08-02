using Data.Model.Dto;
using Data.Model.Entities;
using IService;
using Microsoft.AspNetCore.Mvc;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Shared;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace UserManagement.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class UserController : Controller
    {
        public string path;
        private readonly IUserService _userService;
       
        /*private readonly ApplicationDbContext _context;*/
        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        //1.Api to get all users details.
        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetUsers([FromQuery]int pageNumber, [FromQuery] int pageSize)
        {
            var users = await _userService.GetUsers(pageNumber,pageSize);

            // Use System.Text.Json for JSON serialization with appropriate settings
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            //var json = JsonSerializer.Serialize(users, options);
            // return Content(json, "application/json");
            return new JsonResult(users.Users,options);
 
        }


        [HttpPost("createUser")]
        public async Task<string> CreateUsers([FromForm] UserDetailsDto user)
        {
            return await _userService.CreateUsers(user);
        }

        //3.Api to update the user details.
        [HttpPut("editUsers/{userId}")]
        public async Task<IActionResult> EditUser([FromRoute] string userId, [FromForm] UserDetailsDto userDetails, [FromQuery] bool editPicStatus)
        {
            var result = await _userService.EditUsers(userId, userDetails, editPicStatus);
            if (result)
            {
                return Ok(new { message = "User updated successfully" });
            }
            return BadRequest(new { message = "Failed to update user" });
        }



        //4.Api to Delete the user 
        [HttpDelete("deleteUsers/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {


            var result = await _userService.DeleteUsers(userId);
            if (result)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            return BadRequest(new { message = "Failed to delete user" });
        }



        //5.Api to Filter the users.
        // Criterias : Active / Inactive
        [HttpGet("filterUsers")]
        public async Task<ActionResult<IEnumerable<User>>> FilterUsers([FromQuery] string status)
        {
            var users = await _userService.FilterUsers(status);

            // Use System.Text.Json for JSON serialization with appropriate settings
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            var json = JsonSerializer.Serialize(users, options);
            return Content(json, "application/json");
        }


        //6.Api to Sort the users.
        // Criterias : Age, Joining Date
        [HttpGet("sortUsers")]
        public async Task<ActionResult<IEnumerable<User>>> SortUsers([FromQuery] string sort_order)
        {
            var result= await _userService.SortUsers(sort_order);

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            var json = JsonSerializer.Serialize(result, options);
            return Content(json, "application/json");
        }



        //7.Api to change Password

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePassword)
        {
            if (await _userService.ChangePassword(changePassword))
            {
                return Ok(true);
            }
            return BadRequest(false);
        }


        //8.Api to generate excel 
        [HttpGet("generate-excel")]
        public Task<IActionResult> ExportToExcel()
        {
            return  _userService.ExportToExcel();
        }

        //9.Api to get user by id
        [HttpGet("getUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var users = await _userService.GetUserById(id);

          
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true // Optional: pretty print the JSON
            };

            var json = JsonSerializer.Serialize(users, options);
            return Content(json, "application/json");

        }
    }
}
