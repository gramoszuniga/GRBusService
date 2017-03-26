/* File name: GRRouteStopController.cs
 * Description: Controller for routeStop table.
 *              It creates, read, updates and deletes records for routeStop table.
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
    public class GRRouteStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all stop records for a given route, sends list to Index view under GRRouteStop folder   
        public ActionResult Index(string busRouteCode = "", string routeName = "")
        {
            if (busRouteCode != "")
            {
                Session["busRouteCode"] = busRouteCode;
                Session["routeName"] = routeName;
            }
            else
            {
                if (Session["busRouteCode"] != null)
                {
                    busRouteCode = (string)Session["busRouteCode"];
                    routeName = (string)Session["routeName"];
                }
                else
                {
                    TempData["message"] = "Please select a bus route first to see its stops.";
                    return RedirectToAction("Index", "GRBusRoute");
                }
            }
            var routeStops = db.routeStops.Where(r => r.busRouteCode.Equals(busRouteCode)).Include(r => r.busRoute).Include(r => r.busStop).OrderBy(r => r.offsetMinutes);
            return View(routeStops.ToList());
        }

        // if id is not null and matches a record key in routeStop table, sents route stop found
        // to Details view under GRRouteStop folder
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            Session["stopNumber"] = routeStop.busStopNumber;
            return View(routeStop);
        }

        // creates dropdown lists for bus route code and bus stop number and calls Create view under GRRouteStop folder
        public ActionResult Create()
        {
            if (String.IsNullOrEmpty((string)Session["busRouteCode"]))
            {
                TempData["message"] = "Please select a bus stop first to create a stop for it.";
                return RedirectToAction("Index", "GRBusRoute");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location");
            return View();
        }

        // captures route stop id, bus route code, bus stop number and offset minutes. If they are valid
        // creates dropdown lists for bus route code and bus stop number and creates new route stop and 
        // calls Index view under GRRouteStop folder. Otherwise, calls Create view under GRRouteStop folder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.routeStops.Add(routeStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // if id is not null and matches a record key in routeStop table, creates dropdown lists for bus route code and bus stop number 
        // sends route stop found to Edit view under GRRouteStop folder.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            Session["stopNumber"] = routeStop.busStopNumber;
            return View(routeStop);
        }

        // captures route stop id, bus route code, bus stop number and offset minutes. If they are valid, modifies route stop and 
        // calls Index view under GRRouteStop folder. Otherwise, calls Edit view under GRRouteStop folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeStopId,busRouteCode,busStopNumber,offsetMinutes")] routeStop routeStop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeStop).State = EntityState.Modified;
                db.SaveChanges();
                Session["stopNumber"] = routeStop.busStopNumber;
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeStop.busRouteCode);
            ViewBag.busStopNumber = new SelectList(db.busStops, "busStopNumber", "location", routeStop.busStopNumber);
            return View(routeStop);
        }

        // if id is not null and matches a record key in routeStop table, sends route stop found
        // to Delete view under GRRouteStop folder
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeStop routeStop = db.routeStops.Find(id);
            if (routeStop == null)
            {
                return HttpNotFound();
            }
            Session["stopNumber"] = routeStop.busStopNumber;
            return View(routeStop);
        }

        // removes route stop found from routeStop table and calls Index view under GRRouteStop folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeStop routeStop = db.routeStops.Find(id);
            db.routeStops.Remove(routeStop);
            db.SaveChanges();
            Session["stopNumber"] = routeStop.busStopNumber;
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
