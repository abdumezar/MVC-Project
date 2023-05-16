using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;

		public AccountController(UserManager<ApplicationUser> userManager_, SignInManager<ApplicationUser> signInManager_)
        {
			userManager = userManager_;
			signInManager = signInManager_;
		}


        #region Register

        // Account/Register
        public ActionResult Register() 
		{
            return View();
        }

		[HttpPost]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid) // Server Side Validation
			{
				var user = new ApplicationUser()
				{
					FName = model.FName,
					LName = model.LName,
					UserName = model.Email.Split('@')[0],
					Email = model.Email,
					IsAgree = model.IsAgree
				};

				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Login));
				}

				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
				
			}
			return View(model);
		}

		#endregion

		#region Login

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);
				if (user is not null)
				{
					var flag = await userManager.CheckPasswordAsync(user, model.Password);
					if (flag)
					{
						await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
						return RedirectToAction("Index", "Home");
					}
					ModelState.AddModelError(string.Empty, "Invalid Password!");
				}
				ModelState.AddModelError(string.Empty, "Email is not Exist!");
			}
			return View(model);
		}

		#endregion

		#region Sign Out

		public new async Task<IActionResult> SignOut()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}

		#endregion

		#region Forget Register

		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(model.Email);
				if(user is not null)
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var passwordResetLink = Url.Action("ResetPassword", "Account", new {email = user.Email, token = token}, Request.Scheme);

					// https://localhost:44354/Account/ResetPassword?email=mail@email.com?token=vjkhfsbv9r7bfov
					var email = new Email()
					{
						Subject = "Reset Password",
						To = user.Email,
						Body = passwordResetLink
					};
					EmailSettings.SendEmail(email);
					return RedirectToAction(nameof(CheckYourInbox));
				}
				ModelState.AddModelError(string.Empty, "Email is not Exist!");
			}
			return View(model);
		}

		public IActionResult CheckYourInbox()
		{
			return View();
		}

		#endregion

		#region Reset Password

		public IActionResult ResetPassword(string email, string token)
		{
			TempData["email"] = email;
			TempData["token"] = token;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				string email = TempData["email"] as string;
				string token = TempData["token"] as string;

				var user = await userManager.FindByEmailAsync(email);
				if(user is not null)
				{
					var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

					if (result.Succeeded)
						return RedirectToAction(nameof(Login));

					foreach (var error in result.Errors)
						ModelState.AddModelError(string.Empty, error.Description);
				}
				ModelState.AddModelError(string.Empty, "Email is not Exist");
			}

			return View(model);
		}

		#endregion
	
		
	}
}
