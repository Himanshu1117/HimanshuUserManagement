using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Data.Model.Dto;
using IService;
using Shared;


namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {

            _emailService = emailService;
        }
        //1.Api to send the reset email.
        [HttpPost("send-reset-email")]
        public async Task<OperationResult> SendResetEmail([FromBody] string email)
        {
            return await _emailService.SendResetEmail(email);
        }


        //2.Api to reset the password.
        [HttpPost("reset-password")]
        public async Task<OperationResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            return await _emailService.ResetPassword(resetPasswordDto);
        }

    }
}
