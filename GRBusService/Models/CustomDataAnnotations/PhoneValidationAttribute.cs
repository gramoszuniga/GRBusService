/* File name: PhoneValidationAttribute.cs
 * Description: Custom validation for phone numbers.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using GRBusService.App_GlobalResources;

namespace GRBusService.Models
{
    public class PhoneValidationAttribute : ValidationAttribute
    {
        // Class constructor. Sets up error message
        public PhoneValidationAttribute()
        {
            ErrorMessage = GRTranslations.PhoneValidation;
        }
        
        // Validates a phone number ensuring input matches canadian format
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (string)value != "")
            {
                Regex pattern = new Regex(@"^([^0-9]*\d){10}[^0-9]*$");
                if (!pattern.IsMatch(value.ToString()))
                {
                    return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}