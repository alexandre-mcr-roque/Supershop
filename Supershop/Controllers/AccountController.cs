using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Supershop.Data.Entities;
using Supershop.Helpers;
using Supershop.Models;

namespace Supershop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            if (Request.Query["RegisterSuccess"].FirstOrDefault() == bool.TrueString)
            {
                var model = new LoginViewModel { Username = Request.Query["Username"].FirstOrDefault() ?? string.Empty };
                ModelState.AddModelError(string.Empty, "Account created.");
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Username","Password","RememberMe")]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First()!);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Failed to login.");
            model.Password = ""; // Clear password field
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Register()
        {
            await _userHelper.LogoutAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("FirstName","LastName","Username","Password","ConfirmPassword")]RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username
                    };
                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        model.Password = ""; model.ConfirmPassword = ""; // Clear password field
                        return View(model);
                    }
                    //var loginViewModel = new LoginViewModel
                    //{
                    //    Username = model.Username,
                    //    Password = model.Password,
                    //    RememberMe = false
                    //};
                    //var result2 = await _userHelper.LoginAsync(loginViewModel);
                    //if (result2.Succeeded)
                    //{
                    //    return RedirectToAction("Index", "Home");
                    //}
                    return RedirectToAction("Login", new { RegisterSuccess = true, model.Username });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    model.Password = ""; model.ConfirmPassword = ""; // Clear password field
                    return View(model);
                }
            }
            ModelState.AddModelError(string.Empty, "The user couldn't be created.");
            model.Password = ""; model.ConfirmPassword = ""; // Clear password field
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null) return NotFound();

            var model = new ChangeUserViewModel();
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.PhoneNumber = user.PhoneNumber;
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeUser([Bind("FirstName","LastName","PhoneNumber")]ChangeUserViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {
                    ViewBag.UserMessage = "User updated.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault()?.Description ?? "The user couldn't be updated.");
                }
                return View(model);
            }
            ModelState.AddModelError(string.Empty, "The user couldn't be updated.");
            return View(model);
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ChangeUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Errors.First().Description);
                    return View();
                }
            }
            ModelState.AddModelError(string.Empty, "The password couldn't be updated.");
            return View(); // Do not return password fields
        }
    }
}
