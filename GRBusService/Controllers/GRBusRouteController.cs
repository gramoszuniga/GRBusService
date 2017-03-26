/* File name: GRBusRouteController.cs
 * Description: Controller for busRoute table.
 *              It creates, read, updates and deletes records for busRoute table.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GRBusService.Models;

namespace GRBusService.Controllers
{
    [Authorize]
    public class GRBusRouteController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all bus route records, sends list to Index view under GRBusRoute folder
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.busRoutes.ToList());
        }

        // if id is not null and matches a record key in busRoute table, sends bus route found
        // to Details view under GRBusRoute folder
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // calls Create view under GRBusRoute folder
        public ActionResult Create()
        {
            return View();
        }

        // captures bus route code and name from form. If they are valid, creates new bus route and
        // calls Index view under GRBusRoute folder. Otherwise, calls Edit view under GRBusRoute folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.busRoutes.Add(busRoute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busRoute);
        }

        // if id is not null and matches a record key in busRoute table, sends bus route found
        // to Edit view under GRBusRoute folder
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // captures bus route code and name from form. If they are valid, modifies bus route and
        // calls Index view under GRBusRoute folder. Otherwise, calls Edit view under GRBusRoute folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busRouteCode,routeName")] busRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(busRoute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // if id is not null and matches a record key in busRoute table, sends bus route found
        // to Delete view under GRBusRoute folder
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busRoute busRoute = db.busRoutes.Find(id);
            if (busRoute == null)
            {
                return HttpNotFound();
            }
            return View(busRoute);
        }

        // removes bus route found from busRoute table and calls Index view under GRBusRoute folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            busRoute busRoute = db.busRoutes.Find(id);
            db.busRoutes.Remove(busRoute);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // when disposing, releases database related resources
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}