/* File name: DateNotInFutureAttribute.cs
 * Description: Custom validation for dates.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRBusService.App_GlobalResources;

namespace GRBusService.Models
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        // Class constructor. Sets up error message
        public DateNotInFutureAttribute()
        {
            ErrorMessage = GRTranslations.DateNotInFuture;
        }

        // Validates a date and ensures is not later that the current date
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (DateTime)value > DateTime.Now)
            {
                return new ValidationResult(String.Format(ErrorMessage, validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}