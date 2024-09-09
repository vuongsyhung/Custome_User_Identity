using CodeGenerate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace CodeGenerate.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> userMgr, SignInManager<User> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
        }


        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }



        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password,RememberMe,ReturnUrl")] Login login)
        {
            if (ModelState.IsValid)
            {
                User? user = await userManager.FindByEmailAsync(login.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, login.RememberMe, false);
                    if (result.Succeeded)
                        return Redirect(login.ReturnUrl ?? "/Home/Secured");
                }
                ModelState.AddModelError(nameof(login.Email), "Login Failed: Invalid Email or password");
            }
            return View(login);
        }
    }
}

/*
We have provided false values for both last 3rd and 4th parameters
because we don’t want a persistence cookie for a persistence login
(that remain even after closing the browser),
nor we want to lock the user account when sign-in fails.


Check if the ReturnUrl is not null and is a local URL
if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
{
    return Redirect(ReturnUrl);
}

An Open Redirect Attack occurs when an application redirects users to an external, 
potentially malicious URL. By using the IsLocalUrl method, 
you can ensure that your application only redirects to URLs within your application, 
thereby avoiding such security risks.
*/


/*What is Remember Me Feature in Website Login?
When you log in to a specific website, 
sometimes your web browser will ask if you want it to remember the password for that site, 
or it may say “Remember me?” 
meaning “Do you want this site to hold on to your login credentials?” 
If you click for the browser to remember your password or to remember you, 
the browser will store your login information and 
autofill the login fields each time you go to access that specific site.

In order to implement the Remember Me feature in ASP.NET Identity, 
we use the isPersistent property of the PasswordSignIn method 
when we are log-in to the application. 
All we have to do is to set the isPersistent property in the PasswordSignIn method to true.

Instead of hardcoding the value to true, in real-time applications, 
we need to pass the RememberMe value to the Action method using an HTML checkbox from a view. 
In that case, if the user wants the browser and server to remember the password, 
then he needs to check the RememberMe check else he needs to uncheck the RememberMe checkbox. 
And instead of hardcoding the isPersistent value, we need to set the value as follows.
*/

/*PasswordSignIn(model.Email, model.Password, isPersistent: true, shouldLockout: false);*/