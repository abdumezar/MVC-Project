using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMapper mapper;

        public UserController(UserManager<ApplicationUser> userManager_, SignInManager<ApplicationUser> signInManager_ , IMapper mapper_)
		{
			userManager = userManager_;
			signInManager = signInManager_;
            mapper = mapper_;
        }
		public async Task<IActionResult> Index(string email)
		{
            if (string.IsNullOrEmpty(email))
			{
                var users = await userManager.Users.Select(U => new UserViewModel()
                {
                    Id = U.Id,
                    FName = U.FName,
                    LName = U.LName,
                    Email = U.Email,
                    PhoneNumber = U.PhoneNumber,
                    Roles = userManager.GetRolesAsync(U).Result
                }).ToListAsync();
                return View(users);
			}
			else
			{
				var user = await userManager.FindByEmailAsync(email);
				if(user is not null)
                {
                    var mappedUser = new UserViewModel()
                    {
                        Id = user.Id,
                        FName = user.FName,
                        LName = user.LName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = userManager.GetRolesAsync(user).Result
                    };
				    return View(new List<UserViewModel>(){ mappedUser });
                }
                else
                {
                    return View(Enumerable.Empty<UserViewModel>());
                }
            }
		}


        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var user = await userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var mappedUser = mapper.Map<ApplicationUser, UserViewModel>(user);

            return View(ViewName, mappedUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserViewModel updateUser)
        {
            if (ModelState.IsValid) // server side validation
            {
                try
                {
                    var user = await userManager.FindByIdAsync(id);

                    user.FName = updateUser.FName;
                    user.LName = updateUser.LName;
                    user.PhoneNumber = updateUser.PhoneNumber;
                    user.Email = updateUser.Email;
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    await userManager.UpdateAsync(user);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(updateUser);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);

                await userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error", "Home");
            }

        }

    }
}
