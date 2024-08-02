using IRepository;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Data.Model.Dto;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Data.Context;
using Shared;
using MailKit;
using static Shared.Messages;

namespace Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public EmailRepository(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }



        //1.Method to Send the email to user when it clicks on forgetPassword.
        public async Task<OperationResult> SendResetEmailRepo(String email)
        {
            //Checking if the user with the email exits or not.
            var user = await _context.UpretiUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                var message1 = Messages.GetMessage(MessageType.Message1);
                if (message1 != null)
                {
                    return message1;
                }
            }


            //Genearting a random token 
            var tokenBytes = new byte[64];
            using (var token = RandomNumberGenerator.Create())
            {
                token.GetBytes(tokenBytes);
            }
            var emailToken = Convert.ToBase64String(tokenBytes);


            //Assignment of Random generated token to ResetPasswordToken and adding Expiration time
            user.ResetPasswordToken = emailToken;
            user.ResetExpiryToken = DateTime.Now.AddMinutes(10);

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            //Generating the email configuration.
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Shared", "Template", "EmaiBody.html");
            var templateContent = await File.ReadAllTextAsync(templatePath);
            var resetLink = _config["ResetLink:HtmlLink"];
            var resetLinkWithEmail = $"{resetLink}/?email={email}&code={emailToken}";
            var htmlMessage = templateContent.Replace("{ResetLink}", resetLinkWithEmail);
            string from = _config["ClassConfiguration:From"];
            var subject = "Reset Password";




            //Generating the email
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Himanshu", from));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // sending the email to recipient
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_config["ClassConfiguration:SmtpServer"], Convert.ToInt32(_config["ClassConfiguration:Port"]));//establishing connection
                    client.Authenticate(_config["ClassConfiguration:From"], _config["ClassConfiguration:Password"]);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    throw;
                }

                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }

            var message2 = Messages.GetMessage(MessageType.Message2);
            return message2;

        }





        //2.Method to reset the password.
        public async Task<OperationResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            // Checking if the user with the email exists or not
            var user = await _context.UpretiUsers
                .FirstOrDefaultAsync(u => u.Email == resetPassword.Email);

            if (user == null)
            {
                var message3 = Messages.GetMessage(MessageType.Message3);
                return message3;
            }

            // Accessing the token and expiration time
            var tokenCode = user.ResetPasswordToken;
            var emailTokenExpiry = user.ResetExpiryToken;

            // Checking if the token is valid or not
            if (tokenCode != resetPassword.EmailToken && emailTokenExpiry < DateTime.Now)
            {
                var message4 = Messages.GetMessage(MessageType.Message4);
                return message4;
            }

            // Hashing the password and saving the changes
            user.Password = PasswordHasher.HashPassword(resetPassword.NewPassword);

            //_context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var message5 = Messages.GetMessage(MessageType.Message5);
            return message5;
        }
    }

    }





