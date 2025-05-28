using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.IdentityModel.Tokens;
using Supershop.Data;
using Supershop.Data.Entities;
using Supershop.Helpers;
using Supershop.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Supershop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly ICountryRepository _countryRepository;

        public AccountController(
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            ICountryRepository countryRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _countryRepository = countryRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            var model = new RegisterNewUserViewModel
            {
                Cities = _countryRepository.GetComboCities(0),  // Empty list
                Countries = _countryRepository.GetComboCountries()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("FirstName","LastName","Username","PhoneNumber","Address","CityId","CountryId","Password","ConfirmPassword")]RegisterNewUserViewModel model)
        {
            string errorMessage = "The user couldn't be created.";
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user == null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        City = city
                    };
                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result == IdentityResult.Success)
                    {
                        string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                        string tokenLink = Url.Action("ConfirmEmail","Account",new
                        {
                            userid = user.Id,
                            token = myToken
                        }, protocol: HttpContext.Request.Scheme)!;

                        Response response = _mailHelper.SendEmail(model.Username,"Email Confirmation",
                            "<h1>Email Confirmation</h1>" +
                            "To allow the user, please click on the following link:<br><br>" +
                            $"<a href=\"{tokenLink}\">Confirm Email</a>");
                        if (response.IsSuccess)
                        {
                            ViewBag.Message = "The instructions to allow your user has been sent to email";
                            model.Password = ""; model.ConfirmPassword = ""; // Clear password field
                            model.Cities = _countryRepository.GetComboCities(model.CountryId);
                            model.Countries = _countryRepository.GetComboCountries();
                            return View(model);
                        }
                    }
                }
                else
                {
                    errorMessage = "An account with this email already exists.";
                }
            }
            ModelState.AddModelError(string.Empty, errorMessage);
            model.Password = ""; model.ConfirmPassword = ""; // Clear password field
            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Countries = _countryRepository.GetComboCountries();
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
            if (user == null) return NotFound();

            var cityId = user.CityId.GetValueOrDefault();
            var countryId = (await _countryRepository.GetCityAsync(cityId))?.CountryId ?? 0;
            var model = new ChangeUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CityId = cityId,
                Cities = _countryRepository.GetComboCities(countryId),
                CountryId = countryId,
                Countries = _countryRepository.GetComboCountries()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser([Bind("FirstName","LastName","PhoneNumber","Address","CityId","CountryId")]ChangeUserViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name ?? string.Empty);
            if (user == null) return NotFound();

            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Countries = _countryRepository.GetComboCountries();
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name ?? string.Empty);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateToken([FromBody,Bind("Username,Password")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]!));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };
                        return Created(string.Empty, results);
                    }
                }
            }
            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword([Bind("Email")]RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ViewBag.Error = true;
                    ViewBag.Message = "The email does not correspond to a registered account.";
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var tokenLink = Url.Action("ResetPassword", "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendEmail(model.Email, "Password Reset",
                    "<h1>Password Reset</h1>" +
                    "To reset the password, please click on the following link:<br><br>" +
                    $"<a href=\"{tokenLink}\">Reset password</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to reset your password has been sent to your email";
                }
                else
                {
                    ViewBag.Error = true;
                    ViewBag.Message = "Internal server error. Please try again later.";
                }
                return View(model);
            }
            ViewBag.Error = true;
            ViewBag.Message = "Something wrong happened.";
            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind("Username","Password","ConfirmPassword","Token")]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        ViewBag.Message = "Password reset successfully.";
                        model.Password = ""; model.ConfirmPassword = "";    // Clear password field
                        return View(model);
                    }
                    ViewBag.Error = true;
                    ViewBag.Message = "Error while resetting the password.";
                    model.Password = ""; model.ConfirmPassword = "";    // Clear password field
                    return View(model);
                }
            }
            ViewBag.Error = true;
            ViewBag.Message = "Something wrong happened.";
            model.Password = ""; model.ConfirmPassword = "";    // Clear password field
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View();
        }

        [HttpPost]
        [Route("Account/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);
            return Json(country?.Cities!.OrderBy(c => c.Name));
        }
    }
}
