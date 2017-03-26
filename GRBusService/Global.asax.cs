/* File name: Global.asax.cs
 * Description: Class for appication set up.
 *              It sets application values and sets language at startup.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GRBusService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // Sets up application values upon start
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "GRTranslations";
            DefaultModelBinder.ResourceClassKey = "GRTranslations";
        }

        // Sets up language and culture according to cookie
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (Request.Cookies["language"] != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Request.Cookies["language"].Value);
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Request.Cookies["language"].Value);
            }
        }
    }
}
