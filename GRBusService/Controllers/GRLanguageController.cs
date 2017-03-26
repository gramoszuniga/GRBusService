/* File name: GRLanguageController.cs
 * Description: Controller for changing language and culture.
 *              It shows users available languages and let them pick one.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GRBusService.Controllers
{
    public class GRLanguageController : Controller
    {
        // Sets up a select list with available languages and sents it to ChangeLanguage view under GRLanguage folder
        public ActionResult ChangeLanguage()
        {
            SelectListItem english = new SelectListItem() { Text = "English", Value = "en" };
            SelectListItem spanishChile = new SelectListItem() { Text = "Español (Chile)", Value = "es-cl" };                        
            SelectListItem[] languages = new SelectListItem[] { english, spanishChile };
            if (Request.Cookies["language"] == null)
            {
                english.Selected = true;
            }
            else
            {
                foreach (SelectListItem item in languages)
                {
                    if (item.Value == Request.Cookies["language"].Value)
                    {
                        item.Selected = true;
                    }
                }
            }
            ViewBag.language = new SelectListItem[] { english, spanishChile };
            if (Request.UrlReferrer != null)
            {
                Response.Cookies.Add(new HttpCookie("returnURL", Request.UrlReferrer.PathAndQuery));
            }
            else
            {
                Response.Cookies.Remove("returnUrl");
            }
            return View();
        }

        // Saves chosen languages in a cookie and redirects to previous webpage
        [HttpPost]
        public void ChangeLanguage(string language)
        {
            Response.Cookies.Add(new HttpCookie("language", language));
            if (Request.Cookies["returnURL"] != null)
            {
                Response.Redirect(Request.Cookies["returnURL"].Value);
            }
            else
            {
                Response.Redirect("/");
            }
        }
    }
}