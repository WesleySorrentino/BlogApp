using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApplication.Data;
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

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _toastNotification.AddErrorToastMessage("Could not find the user with the email or password provided.");
                return View();
            }

            //sign in
            var signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);

            if (signInResult.Succeeded)
            {
                _toastNotification.AddSuccessToastMessage($"Successfully Logged in as {user.UserName}", new ToastrOptions
                { 
                    CloseButton = true
                });

                return RedirectToAction("Index","Home");
            }

            _toastNotification.AddErrorToastMessage("An Error Occured");

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };
            var result = await _userManager.CreateAsync(user, password);

            if (!_appDbContext.Users.Any(u=>u.Email == user.Email))
            {
                if (result.Succeeded)
                {
                    //sign user here

                    var currentUser = await _userManager.FindByNameAsync(username);



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
            await _signInManager.SignOutAsync();

            _toastNotification.AddSuccessToastMessage("Successfully Logged out");

            return RedirectToAction("Index", "Home");
        }
    }
}
