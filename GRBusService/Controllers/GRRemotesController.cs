/* File name: GRRemotesController.cs
 * Description: Controller for remotes.
 *              It performs various validations.
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.App_GlobalResources;
using GRBusService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GRBusService.Controllers
{
    public class GRRemotesController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // Validates provice code. It ensures it exist and it is in a valid format
        public JsonResult ProvinceCodeValidator(string provinceCode)
        {
            provinceCode = provinceCode.ToUpper();
            if (provinceCode.Length != 2 ||
                provinceCode.ElementAt(0) < 65 || provinceCode.ElementAt(0) > 90 ||
                provinceCode.ElementAt(1) < 65 || provinceCode.ElementAt(1) > 90)
            {
                return Json(GRTranslations.provinceCodeMustBeTwoLettersLong, JsonRequestBehavior.AllowGet);
            }
            try
            {
                province province = db.provinces.Find(provinceCode);
                if (province == null)
                {
                    return Json(GRTranslations.provinceCodeDoesNotExist, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception error)
            {
                return Json(GRTranslations.ErrorValidatingProvinceCode + error.GetBaseException().Message, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}