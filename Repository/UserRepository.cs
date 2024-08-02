using Data.Context;
using Data.Model.Dto;
using Data.Model.Entities;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Shared;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Microsoft.Data.SqlClient;
using System.IO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Drawing.Printing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Repository
{
    public class UserRepository : IUserRepository
    {

        EncryptionShare es =new EncryptionShare();
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public UserRepository(ApplicationDbContext context, IConfiguration congfig)
        {
            _context = context;
            _config = congfig;
        }

        //1.method to get all the users
        public async Task<(IEnumerable<User> Users, int TotalUsers)> GetUsers(int pageNumber,int pageSize)
        { 
            try
            {
                var result = _context.UpretiUsers
                        .Where(user => user.IsDeleted == false)
                        .Include(u => u.Addresses)
                        .AsQueryable();

                var totalUsers = await result.CountAsync();
                var users = await result
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.Phone = es.Decrypt(user.Phone, _config["Encryption:Key"], _config["Encryption:IV"]);
                    user.Email = es.Decrypt(user.Email, _config["Encryption:Key"], _config["Encryption:IV"]);
                }

                return (users, totalUsers);
            }
          
                
            catch (Exception ex)
            {
                
                throw new ApplicationException("An error occurred while retrieving users.", ex);
            }

           
        }




        //2.method  to edit the users.

        public async  Task<bool> EditUsers(string id, UserDetailsDto userDetails, bool status)
        {
            // Find the user by ID
            var user = await _context.UpretiUsers.FirstOrDefaultAsync(x => x.User_Id == Convert.ToInt32(id));

            if (user == null)
            {
                return false;
            }

            if (status == false)
            {
                // Update other user details if needed
                _context.UpretiUsers.Update(user);
            }
            else
            {
                // Handle profile image update if provided
                if (userDetails.userImage != null && userDetails.userImage.Length > 0)
                {
                    var fileName = Path.GetFileName(userDetails.userImage.FileName);
                    
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Shared", "wwwroot", "images", fileName);
                    var filePath = Path.Combine(folderPath, fileName);
                  
                    // Ensure the directory exists
                    Directory.CreateDirectory(folderPath);

                    try
                    {
                        // Save the file to disk
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await userDetails.userImage.CopyToAsync(stream);
                        }

                        // Set the profile image path in the user object
                        userDetails.ImagePath = $"/images/{fileName}";
                        user.ImagePath = userDetails.ImagePath;

                        // Update user entity with new image path and other details
                        _context.UpretiUsers.Update(user);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions, such as file I/O errors
                        return false;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions related to database operations
                return false;
            }
        }



        //3.method to Delete user.
        public async Task<bool> DeleteUsers(int id)
        {
            var user = _context.UpretiUsers.FirstOrDefault(x => x.User_Id == id);

            /* if (user == null)
             {

                 throw new InvalidOperationException($"User with ID {id} not found.");

             }*/

            var affectedRows = await _context.Database.ExecuteSqlRawAsync(
                "EXEC SetUserDeletedStatus @Id",
                new SqlParameter("@Id", id));
                _context.SaveChanges();
            var imagePath = user.ImagePath;
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error)
                    throw new InvalidOperationException($"Failed to delete image file: {ex.Message}", ex);
                }
            }
            return affectedRows>0;
        }


        //4.method to FilterUsers.
        public async Task<IEnumerable<User>> FilterUsers(string status)
        {
            bool isActive;
            if (status == "Active")
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }
            var user = _context.UpretiUsers.AsQueryable();
            return await _context.UpretiUsers
                 .Where(user => user.IsDeleted == false && user.IsActive == isActive)
                 .Include(u => u.Addresses)
                 .ToListAsync();
          

        }

        //5.method to SortUsers.

        public async Task<IEnumerable<User>> SortUsers(string sort_order)
        {
            var a = _context.UpretiUsers.AsQueryable();

            if (sort_order == "age")
            {
                a = a.OrderBy(u => DateTime.Now.Year - (u.DOB).Year);
            }
            else if (sort_order == "name")
            {
                a = a.OrderBy(u => u.FirstName);
            }

            return await a.Include(u => u.Addresses).ToListAsync();
        }


       /// <summary>
       /// method to get user by id
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
       /// <exception cref="Exception"></exception>
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.UpretiUsers
        .Include(u => u.Addresses) // Include related entities
        .FirstOrDefaultAsync(x => x.User_Id == id); 
            if (user != null && !user.IsDeleted) // Simplify check for IsDeleted
            {
                return user;
            }
            else
            {
                throw new Exception("User not found or user is deleted."); // Provide a meaningful exception message
            }
        }

        /// <summary>
        /// method to  create the users
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> CreateUsers(UserDetailsDto user)


        {

            var existingUser = await _context.UpretiUsers
        .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                string BadRequest = "Email already exists.";
                return BadRequest;
            }
            else
            {
                string ImagePath = null;
                if (user.userImage != null && user.userImage.Length > 0)
                {
                    var fileName = Path.GetFileName(user.userImage.FileName);
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Shared", "wwwroot", "images", fileName);

                    var filePath = Path.Combine(folderPath, fileName);
                    // Ensure the directory exists
                    Directory.CreateDirectory(folderPath);

                    // Save the file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        user.userImage.CopyToAsync(stream);
                    }
                    // Set the profile image path in the user object
                    ImagePath = $"/images/{fileName}";

                }

                bool a = true;
                try
                {

                    if (user == null)
                    {
                        string x = "false";

                        return x;
                        /*  return new OperationResult
                          {
                              IsSuccess = false,
                              ErrorMessage = "Invalid user data"
                          };*/
                    }
                    var PhoneEncrypt = es.Encrypt(user.Phone, _config["Encryption:Key"], _config["Encryption:IV"]);
                    var EmailEncrypt = es.Encrypt(user.Email, _config["Encryption:Key"], _config["Encryption:IV"]);

                    var userDetailsEntity = new User
                    {
                        FirstName = user.FirstName,
                        MiddleName = user.MiddleName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Email = EmailEncrypt,
                        DateOfJoining = user.DateOfJoining,
                        DOB = user.DOB,
                        Phone = PhoneEncrypt,
                        AlternatePhone = user.AlternatePhone,
                        ImagePath = ImagePath,
                        Password = PassworDGenerator.GenerateRandomPassword(),
                        IsActive = false,
                        IsDeleted = false,
                        Created_at = DateTime.UtcNow,
                        Addresses = user.Address.Select(a => new Address
                        {
                            City = a.City,
                            State = a.State,
                            Country = a.Country,
                            ZipCode = a.ZipCode,
                            AId = Convert.ToInt32(a.AId)
                        }).ToList()
                    };
                    // Save user to database
                    await _context.UpretiUsers.AddAsync(userDetailsEntity);
                    await _context.SaveChangesAsync();
                    a = true;


                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating user: {ex.Message}");
                    return "error";
                    /* new OperationResult
                     /* {
                        IsSuccess = false,
                          ErrorMessage = $"Error creating user: {ex.Message}"
                      };*/
                }

                if (a == true)
                {
                    var users = await _context.UpretiUsers.FirstOrDefaultAsync(u => u.Email == user.Email);

                    var tokenBytes = new byte[64];
                    using (var range = RandomNumberGenerator.Create())
                    {
                        range.GetBytes(tokenBytes);
                    }

                    var emailToken = Convert.ToBase64String(tokenBytes);
                    users.ResetPasswordToken = emailToken;
                    users.ResetExpiryToken = DateTime.Now.AddMinutes(10);

                    _context.Entry(users).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Shared", "Template", "ChangePassword.html");
                    var templateContent = await File.ReadAllTextAsync(templatePath);
                    var confirmationLink = _config["ResetLink:ConfirmationLink"];
                    var changeLinkWithEmail = $"{confirmationLink}?email={users.Email}?code={emailToken}";
                    var htmlMessage = templateContent.Replace("{ResetLink}", changeLinkWithEmail)
                                                       .Replace("{UserName}", users.FirstName)
                                                       .Replace("{Password}", users.Password);
                    string from = _config["ClassConfiguration:From"];
                    var subject = "Change Password";



                    var emailMessage = new MimeMessage();


                    emailMessage.From.Add(new MailboxAddress("Himanshu", from));
                    emailMessage.To.Add(new MailboxAddress("", users.Email));
                    emailMessage.Subject = subject;
                    var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
                    emailMessage.Body = bodyBuilder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        try
                        {
                            client.Connect(_config["ClassConfiguration:SmtpServer"], Convert.ToInt32(_config["ClassConfiguration:Port"]), true);//establishing connection
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
                }
                string ab = "success";
                return ab;
            }
        }

        //7.method to change the password.
        public async Task<bool> ChangePassword(ChangePasswordDto changePassword)
        {
            var user = _context.UpretiUsers.FirstOrDefaultAsync(u => u.Email == changePassword.Email);
            if (user == null)
            {
                return false;
            }

            var userResult = await user; // Await the task
            if (userResult != null)
            {
                var tokenCode = userResult.ResetPasswordToken;

                var emailTokenExpiry = userResult.ResetExpiryToken;

                if (tokenCode != changePassword.EmailToken || emailTokenExpiry < DateTime.Now)
                {
                    return false;
                }
                userResult.IsActive = true; 
                userResult.Password = PasswordHasher.HashPassword(changePassword.NewPassword);
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChangesAsync();

                return true;
            }
            else
            {
                // Handle the case where userResult is null
                throw new InvalidOperationException("User result is null.");
            }

        }



        //8.method to export excel
        public async Task<IActionResult> ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var users = await _context.UpretiUsers.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Adding Header
                worksheet.Cells[1, 1].Value = "FirstName";
                worksheet.Cells[1, 2].Value = "MiddleName";
                worksheet.Cells[1, 3].Value = "LastName";
                worksheet.Cells[1, 4].Value = "Gender";
                worksheet.Cells[1, 5].Value = "DateOfJoining";
                worksheet.Cells[1, 6].Value = "DOB";
                worksheet.Cells[1, 7].Value = "Email";
                worksheet.Cells[1, 8].Value = "Phone";
                worksheet.Cells[1, 9].Value = "AlternatePhone";

                // Adding Data
                for (int i = 0; i < users.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = users[i].FirstName;
                    worksheet.Cells[i + 2, 2].Value = users[i].MiddleName;
                    worksheet.Cells[i + 2, 3].Value = users[i].LastName;
                    worksheet.Cells[i + 2, 4].Value = users[i].Gender;
                    worksheet.Cells[i + 2, 5].Value = users[i].DateOfJoining.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 6].Value = users[i].DOB.ToString("yyyy-MM-dd");
                    worksheet.Cells[i + 2, 7].Value = users[i].Email;
                    worksheet.Cells[i + 2, 8].Value = users[i].Phone;
                    worksheet.Cells[i + 2, 9].Value = users[i].AlternatePhone;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"Users-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.xlsx";

                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = excelName
                };
            }
        }

       
    }
}

