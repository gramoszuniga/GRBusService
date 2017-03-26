/* File name: GRDriverMetadata.cs
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GRBusService.Models
{
    // Partial class for driver table. It implements IValidatableObject Validate method.
    [MetadataType(typeof(GRDriverMetadata))]
    public partial class driver : IValidatableObject
    {
        private BusServiceContext db = new BusServiceContext();

        // Formats various field before being written to the driver table.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            firstName = GRValidations.Capitalise(firstName);
            lastName = GRValidations.Capitalise(lastName);
            fullName = lastName + ", " + firstName;
            if (provinceCode != null)
            {
                provinceCode = provinceCode.ToUpper();
            }
            postalCode = GRValidations.RemoveSpaces(postalCode).ToUpper();
            if (postalCode.Length == 6)
            {
                postalCode = postalCode.Insert(3, " ");
            }
            homePhone = GRValidations.FormatCanadianPhone(homePhone);
            if (workPhone != null)
            {
                workPhone = GRValidations.FormatCanadianPhone(workPhone);
            }
            yield return ValidationResult.Success;
        }
    }

    // Metadata class for driver table. Specifies data annotations for driver table fields.
    public class GRDriverMetadata
    {
        [Display(Name = "driverId", ResourceType = typeof(GRTranslations))]
        public int driverId { get; set; }

        [Required(ErrorMessageResourceName="Required", ErrorMessageResourceType = typeof(GRTranslations))]
        [Display(Name = "firstName", ResourceType = typeof(GRTranslations))]
        public string firstName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(GRTranslations))]
        [Display(Name = "lastName", ResourceType = typeof(GRTranslations))]
        public string lastName { get; set; }

        [Display(Name = "fullName", ResourceType = typeof(GRTranslations))]
        public string fullName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(GRTranslations))]
        [PhoneValidation]
        [Display(Name = "homePhone", ResourceType = typeof(GRTranslations))]
        public string homePhone { get; set; }

        [Display(Name = "workPhone", ResourceType = typeof(GRTranslations))]
        [PhoneValidation]
        public string workPhone { get; set; }

        [Display(Name = "street", ResourceType = typeof(GRTranslations))]
        public string street { get; set; }

        [Display(Name = "city", ResourceType = typeof(GRTranslations))]
        public string city { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(GRTranslations))]
        [Display(Name = "postalCode", ResourceType = typeof(GRTranslations))]
        [PostalCodeValidation]
        public string postalCode { get; set; }

        [Remote("ProvinceCodeValidator", "GRRemotes")]
        [Display(Name = "provinceCode", ResourceType = typeof(GRTranslations))]
        public string provinceCode { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(GRTranslations))]
        [DateNotInFuture]
        [Display(Name = "dateHired", ResourceType = typeof(GRTranslations))]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime dateHired { get; set; }
    }
}