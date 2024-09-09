using Microsoft.AspNetCore.Identity;
using CodeGenerate.Models;

namespace CodeGenerate.CustomIdentity
{
    public class CustomUsernameEmailPolicy : UserValidator<User>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);
            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            if (user.UserName == "Hung")
            {
                errors.Add(new IdentityError
                {
                    Description = "Hung cannot be used as a user name"
                });
            }

            if (!user.Email.ToLower().EndsWith("@gmail.com"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Only gmail.com email addresses are allowed"
                });
            }
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
