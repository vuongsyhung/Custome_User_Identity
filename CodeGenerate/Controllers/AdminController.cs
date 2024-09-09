using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeGenerate.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CodeGenerate.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<User> _userManager;
        private IPasswordHasher<User> _passwordHasher;
        //
        private IPasswordValidator<User> _passwordValidator;
        private IUserValidator<User> _userValidator;

        public AdminController(
            UserManager<User> userManager, 
            IPasswordHasher<User> passwordHasher, 
            IPasswordValidator<User> passwordVal, 
            IUserValidator<User> userValid)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;

            _passwordValidator = passwordVal;
            _userValidator = userValid;
        }

        /*to use my Custom Validation policies for Email and Password,
        we are using the IPasswordValidator and IUserValidator objects
        as in the code below:
        validEmail = await userValidator.ValidateAsync(userManager, user);

        validates the email according to the email policies 
        (both given in 'Program.cs' & 'CustomUsernameEmailPolicy.cs')

        validPass = await passwordValidator.ValidateAsync(userManager, user, password);

        validates the password according to the password policies 
        (both given in 'Program.cs' & 'CustomPasswordPolicy.cs')*/

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,Password,Occupation")] UserInput userInput)
        {
            if (ModelState.IsValid)
            {
                User appUser = new User
                {
                    UserName = userInput.Name,
                    Email = userInput.Email,
                    Occupation = userInput.Occupation,
                };                

                IdentityResult result = await _userManager.CreateAsync(appUser, userInput.Password.Trim());

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(userInput);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            User user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            UserInput userInput = new UserInput
            {
                Email = user.Email,
                Name = user.UserName.Trim(),
                Password = null
            };
            return View(userInput);
        }

       /* // POST: Admin/Edit/5       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Email,Password")] UserInput userInput)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(userInput.Name))
                    user.UserName = userInput.Name;
                else
                    ModelState.AddModelError("", "Name cannot be empty");

                if (!string.IsNullOrEmpty(userInput.Email))
                    user.Email = userInput.Email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(userInput.Password))
                    user.PasswordHash = _passwordHasher.HashPassword(user, userInput.Password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(userInput.Email) && !string.IsNullOrEmpty(userInput.Name) && !string.IsNullOrEmpty(userInput.Password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }*/

        // POST: Admin/Edit/5       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name,Email,Password")] UserInput userInput)
        {
            User user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.UserName = userInput.Name;
                user.Email = userInput.Email;

                IdentityResult validEmail = null;
                if (!string.IsNullOrEmpty(userInput.Email))
                {
                    validEmail = await _userValidator.ValidateAsync(_userManager, user);
                    if (validEmail.Succeeded)
                        user.Email = userInput.Email;
                    else
                        Errors(validEmail);
                }
                else
                    ModelState.AddModelError("", "Email cannot be empty");


                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(userInput.Password))
                {
                    validPass = await _passwordValidator.ValidateAsync(_userManager, user, userInput.Password);
                    if (validPass.Succeeded)
                        user.PasswordHash = _passwordHasher.HashPassword(user, userInput.Password);
                    else
                        Errors(validPass);
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (validEmail != null && validPass != null && validEmail.Succeeded && validPass.Succeeded)
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");

            return View(userInput);
        }


        // Errors
        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

    }
}
