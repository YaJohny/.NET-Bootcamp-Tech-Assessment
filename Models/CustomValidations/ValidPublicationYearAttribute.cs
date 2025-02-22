using System;
using System.ComponentModel.DataAnnotations;

namespace Models.CustomValidations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class ValidPublicationYearAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			// If the value is null, let [Required] handle it if needed  
			if (value == null)
				return ValidationResult.Success;

			if (value is int year)
			{
				int currentYear = DateTime.Now.Year;
				if (year < 1000 || year > currentYear)
				{
					// Error message can include the dynamic current year  
					return new ValidationResult($"Publication year must be between 1000 and {currentYear}.");
				}

				return ValidationResult.Success;
			}

			// If the value isn't an integer, return a validation error.  
			return new ValidationResult("Invalid publication year value.");
		}
	}
}
