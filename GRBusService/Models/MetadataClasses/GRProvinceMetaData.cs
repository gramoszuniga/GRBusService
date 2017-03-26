/* File name: GRProvinceMetaData.cs
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GRBusService.Models
{
    // Partial class for province table.
    [MetadataType(typeof(GRProvinceMetaData))]
    public partial class province
    {

    }

    // Metadata class for province table. Specifies data annotations for province table fields.
    public class GRProvinceMetaData
    {
        public string provinceCode { get; set; }

        [Display(Name = "provinceName", ResourceType = typeof(GRTranslations))]
        public string name { get; set; }
        
        public string countryCode { get; set; }

        public string taxCode { get; set; }

        public double taxRate { get; set; }

        public string capital { get; set; }
    }
}