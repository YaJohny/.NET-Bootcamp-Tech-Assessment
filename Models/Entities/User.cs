using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class User : IdentityUser
    {
		[Required]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Email address is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public override string? Email
		{
			get => base.Email;
			set => base.Email = value;
		}
	}
}
