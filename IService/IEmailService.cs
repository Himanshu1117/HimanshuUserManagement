using Data.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace IService
{
    public interface IEmailService
    {
        
        //1.Method to Send the email to user when it clicks on forgetPassword.
        public Task<OperationResult> SendResetEmail(String email); 


        //2.Method to Reset the Password
        public Task<OperationResult> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
