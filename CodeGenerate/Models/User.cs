using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerate.Models
{
    public class User : IdentityUser
    { 
        [MaxLength(100)]
        public required string Occupation {  get; set; }
    }
}
