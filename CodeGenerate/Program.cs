using CodeGenerate.Controllers;
using CodeGenerate.CustomIdentity;
using CodeGenerate.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HungDbContext>(db => db.UseSqlServer(builder.Configuration.GetConnectionString("HungConnectionString")));
builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<HungDbContext>().AddDefaultTokenProviders();
//configure the password Policy in the Program.cs class by using IdentityOptions object
//configure the Identity Username and Email Policy
builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Password.RequiredLength = 8;
    opts.Password.RequireLowercase = true;
    //
    opts.User.RequireUniqueEmail = true;
    opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    //By setting the RequireUniqueEmail property to true,
    //we enforced that no 2 users can be registered in Identity with the same email address.
    //the AllowedUserNameCharacters.
    //This sets a list of characters
    //that can only be used when creating a username in Identity.
});
//the CustomUsernameEmailPolicy
//custome Password Policy
builder.Services.AddTransient<IPasswordValidator<User>, CustomPasswordPolicy>();
builder.Services.AddTransient<IUserValidator<User>, CustomUsernameEmailPolicy>();
/*builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/GiGiDo/Login");*/
//
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
    options.SlidingExpiration = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();


//ASP.NET Core Identity default Login URL is /Account/Login
//and here users are redirected to authenticate themselves.
//Here a login form is presented to the users for performing the login procedure in Identity.
//If you want to change this login URL then go to the Program class and add the below code to it:

//builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Authenticate/Login");


/*The SlidingExpiration is set to true 
    to instruct the handler 
    to re-issue a new cookie 
    with a new expiration time any time it processes a request 
    which is more than halfway through the expiration window.*/
