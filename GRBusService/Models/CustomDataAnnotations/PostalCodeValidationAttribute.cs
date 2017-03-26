/* File name: PostalCodeValidationAttribute.cs
 * Description: Custom validation for postal code.
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GRBusService.Models
{
    public class PostalCodeValidationAttribute : ValidationAttribute
    {
        // Class constructor. Sets up error message
        public PostalCodeValidationAttribute()
        {
            ErrorMessage = GRTranslations.PostalCodeValidation;
        }

        // Validates postal code ensuring input matches canadian postal code format
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null && (string)value != "")
            {
                Regex pattern = new Regex(@"^[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy]{1}\d{1}[A-Za-z]{1} *\d{1}[A-Za-z]{1}\d{1}$");
                if (!pattern.IsMatch(value.ToString().Trim()))
                {
                    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}