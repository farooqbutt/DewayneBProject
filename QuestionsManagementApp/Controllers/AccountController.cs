using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionsManagementApp.Data;
using QuestionsManagementApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestionsManagementApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext context;
        public AccountController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public int UserDepartmentReturner()
        {
            return context.ApplicationUsers.Where(a => a.Email == User.Identity.Name).FirstOrDefault().UserDepartment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="Supervisor, Manager")]
        public IActionResult UserList()
        {
            var userlist = context.ApplicationUsers.Where(a => a.UserDepartment == UserDepartmentReturner()).ToList();
            var RealUsers = new List<UserViewModel>();
            foreach (var item in userlist)
            {
                var Roles = context.UserRoles.Where(a => a.UserId == item.Id).ToArray();
                var rolename = context.Roles.Find(Roles[0].RoleId).Name;
                var useritem = new UserViewModel
                {
                    Username = item.UserName,
                    Roles = rolename
                };
                RealUsers.Add(useritem);
            }
            return Ok(RealUsers);
        }

        [HttpPost]
        public IActionResult DeleteUser(string email)
        {
            var usertoDelete = context.Users.Where(a => a.Email == email).FirstOrDefault();
            context.Remove(usertoDelete);
            context.SaveChanges();
            return Ok(new { message = "User Delete Successfully" });
        }
    }
}
