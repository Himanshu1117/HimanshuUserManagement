using IService;
using IRepository;
using Data.Model.Dto;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Service
{
    public  class EmailService:IEmailService
    {

        private readonly IEmailRepository _repo;
        public EmailService(IEmailRepository repo)
        {
            _repo = repo;
        }

        //1.Method to Send the email to user when it clicks on forgetPassword.
        public async Task<OperationResult> SendResetEmail(String email)
        { 
            return await _repo.SendResetEmailRepo(email); 
        }


        //2.Method to Reset the Password
        public Task<OperationResult> ResetPassword(ResetPasswordDto resetPassword)
        
        {
            return _repo.ResetPassword(resetPassword);  
        }
    }
}
