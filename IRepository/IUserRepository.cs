using Data.Model.Dto;
using Data.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IRepository
{
    public interface IUserRepository
    {

        public Task<(IEnumerable<User> Users, int TotalUsers)> GetUsers(int pageNumber, int pageSize);

        //method to edit the user
        public Task<bool> EditUsers(string id, UserDetailsDto userDetails, bool status);

        //method to delete the user
        public Task<bool> DeleteUsers(int id);

        //method to filter the user
        public Task<IEnumerable<User>> FilterUsers(string status);

        //  method to sort the user
        public Task<IEnumerable<User>> SortUsers(String sort_order);

        //method to post the user image
        // public Task<string> AddUsers(IFormFile image);

        //method to create the user 

        // public Task<string> CreateUsers(UserDto user, string path);

        public Task<string> CreateUsers(UserDetailsDto user);

        public Task<User> GetUserById(int id);
        // method to change password
        public Task<bool> ChangePassword(ChangePasswordDto changePassword);


        //method to export excel
        public Task<IActionResult> ExportToExcel();

    }
}
