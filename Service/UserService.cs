using Data.Model.Entities;
using IService;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Data.Model.Dto;
using Microsoft.AspNetCore.Hosting;
using Shared;
using Microsoft.AspNetCore.Http;
namespace Service
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepo;


        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        //1.method to get the details of all users
        public Task<(IEnumerable<User> Users, int TotalUsers)> GetUsers(int pageNumber, int pageSize)
        {
            return _userRepo.GetUsers(pageNumber, pageSize);
        }

        //2.method to get the edit the user.

        public Task<bool> EditUsers(string id,UserDetailsDto userDetails, bool status)
        {
            return _userRepo.EditUsers(id, userDetails,status);
        }

        //3.method to delete the user.
        public Task<bool> DeleteUsers(int id)
        {
             return _userRepo.DeleteUsers(id);
        }

        //4.method to filter the user.
        public Task<IEnumerable<User>> FilterUsers(string status)
        {
            return _userRepo.FilterUsers(status);
        }

        //5.method to sort the user.

        public Task<IEnumerable<User>> SortUsers(String sort_name)
        {
            return _userRepo.SortUsers(sort_name);
        }

  

        public Task<string> CreateUsers(UserDetailsDto user)
        {
            return _userRepo.CreateUsers(user);
        }

        public Task<User> GetUserById(int id)
        {
            return _userRepo.GetUserById(id);
        }
        //7.method to change password
        public Task<bool> ChangePassword(ChangePasswordDto changePassword)
        {
            return _userRepo.ChangePassword(changePassword);
        }

        //8.method to export excel
        public  Task<IActionResult> ExportToExcel()
        {
            return  _userRepo.ExportToExcel();
        }

    }
}
