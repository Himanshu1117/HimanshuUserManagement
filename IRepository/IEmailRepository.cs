using Data.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace IRepository
{
    public  interface IEmailRepository
    {
        //1.Method to Send the email to user when it clicks on forgetPassword.
        public Task<OperationResult> SendResetEmailRepo(string email);


        //2.Method to Reset the Password
        public Task<OperationResult> ResetPassword(ResetPasswordDto resetPassword);
    }
}
