using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApplication.Data;
using DataLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;

namespace BlogApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IToastNotification _toastNotification;
        private readonly AppDbContext _appDbContext;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IToastNotification toastNotification, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _toastNotification = toastNotification;
            _appDbContext = appDbContext;
        }

        [Authorize]
        public async Task<IActionResult> Manage()
        {   
            //Gets Current User
            var user = await _userManager.GetUserAsync(User);
           
            if (user.Id == null)
            {
                _toastNotification.AddErrorToastMessage("Error Occured while trying to redirect to Manage Page");

                return RedirectToAction("Index", "Home");
            }

            //Provide the user to Manage Model
            var manageUser = new UserManageModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            return View(manageUser);
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string currentPassword, string confirmCurrentPassword, string newPassword)
        {
            //Gets Current User
            var user = await _userManager.GetUserAsync(User);            

            //Checks if any of the posted fields are null
            if (currentPassword == null && confirmCurrentPassword == null && newPassword == null)
            {
                return View();
            }

            //Checks if passwords match
            if (currentPassword == confirmCurrentPassword)
            {
                //Update users passwords
                await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                _toastNotification.AddSuccessToastMessage("Successfully changed password");

                return RedirectToAction("Index", "Home");
            }

            _toastNotification.AddErrorToastMessage("Error Occured: Could not Update Password </br> Please Check current Passwords Match!");

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            if (email == null || password == null)
            {
                return View();
            }

            //finds user matching provided email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _toastNotification.AddErrorToastMessage("Could not find the user with the email or password provided.");

                return View();
            }

            //sign in user
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (signInResult.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage($"Successfully Logged in as {user.UserName}", new ToastrOptions
                { 
                    CloseButton = true
                });

                return RedirectToAction("Index","Home");
            }

            _toastNotification.AddErrorToastMessage($"An Error Occured: Email or Password doesn't exist");

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            //Creates a new User model
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };

            //adds to database
            var result = await _userManager.CreateAsync(user, password);

            if (!_appDbContext.Users.Any(u=>u.Email == user.Email))
            {
                if (result.Succeeded)
                {
                    //sign user here
                    var currentUser = await _userManager.FindByNameAsync(username);

                    //Add role to user
                    await _userManager.AddToRoleAsync(currentUser, "User");                   

                    var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

                    if (signInResult.Succeeded)
                    {
                        _toastNotification.AddSuccessToastMessage($"Successfully Created an Account!");

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        _toastNotification.AddErrorToastMessage($"{item.Description}");
                    }

                    return View();
                }
            } else
            {
                _toastNotification.AddErrorToastMessage("A User with the email provided already exists!");

                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            //Signs user out
            await _signInManager.SignOutAsync();

            _toastNotification.AddSuccessToastMessage("Successfully Logged out");

            return RedirectToAction("Index", "Home");
        }
    }
}
