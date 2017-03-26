/* File name: GRBusStopController.cs
 * Description: Controller for busStop table.
 *              It creates, read, updates and deletes records for busStop table.
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
    public class GRBusStopController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all bus stop records, sends list to Index view under GRBusStop folder order by busStopNumber or location accordingly
        public ActionResult Index(string orderBy = "")
        {
            if (orderBy.Equals("location"))
            {
                return View(db.busStops.ToList().OrderBy(bs => bs.location));
            }
            return View(db.busStops.ToList().OrderBy(bs => bs.busStopNumber));
        }

        // if id is not null and matches a record key in busStop table, sends bus stop found
        // to Details view under GRBusStop folder
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // calls Create view under GRBusStop folder
        public ActionResult Create()
        {
            return View();
        }

        // captures bus stop number, location and if going downtown or no. If they are valid, generates hash value,
        // creates new bus stop and calls Index view under GRBusStop folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "busStopNumber,location,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.locationHash = GetHashValue(busStop.location);
                db.busStops.Add(busStop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(busStop);
        }

        // if id is not null and matches a record key in busStop table, sends bus stop found
        // to Edit view under GRBusStop folder. Otherwise, calls Edit view under GRBusStop folder.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // captures bus stop number, location and if going downtown or no. If they are valid, generates 
        // hash value, modifies bus stop and calls Index view under GRBusStop folder. Otherwise, calls 
        // Edit view under GRBusStop folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "busStopNumber,location,goingDowntown")] busStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.locationHash = GetHashValue(busStop.location);
                db.Entry(busStop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        // if id is not null and matches a record key in busStop table, sends bus stop found
        // to Delete view under GRBusStop folder
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            busStop busStop = db.busStops.Find(id);
            if (busStop == null)
            {
                return HttpNotFound();
            }
            return View(busStop);
        }

        // removes bus stop found from busStop table and calls Index view under GRBusStop folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            busStop busStop = db.busStops.Find(id);
            db.busStops.Remove(busStop);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // if a bus stop number was given and matches a record in the busStop table, finds all routes for the provided bus stop
        public ActionResult RouteSelector(int busStopNumber = 0)
        {
            if (busStopNumber == 0)
            {
                TempData["message"] = "Please select a bus stop first to see its routes.";
                return RedirectToAction("Index");
            }
            busStop busStop = db.busStops.Find(busStopNumber);
            if (busStop == null)
            {
                TempData["message"] = "Sorry, there is no bus stop with that number.";
                return RedirectToAction("Index");
            }
            Session["busStopNumber"] = busStopNumber;
            Session["location"] = busStop.location;
            var routeStops = db.routeStops.Where(rs => rs.busStopNumber == busStopNumber).Include(rs => rs.busRoute);
            if (routeStops.Count() == 0)
            {
                TempData["message"] = "No routes uses stop " + Session["busStopNumber"] + "-" + Session["location"] + ".";
                return RedirectToAction("Index");
            }
            if (routeStops.Count() == 1)
            {
                int routeStopId = routeStops.Select(rs => rs.routeStopId).Single();
                return RedirectToAction("RouteStopSchedule", "GRRouteSchedule", new { routeStopId = routeStopId });
            }
            ViewBag.routeStopId = new SelectList(routeStops.Select(rs => new { rs.routeStopId, rs.busRoute.routeName }).OrderBy(rs => rs.routeName).ToList(), "routeStopId", "routeName");
            return View();
        }

        // redirects route stop id from dropdownlist in RouteSelector view to RouteStopSchedule action in GRRouteSchedule controller 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RouteSelector(string routeStopId = "")
        {
            return RedirectToAction("RouteStopSchedule", "GRRouteSchedule", new { routeStopId = routeStopId });
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

        // generates hash value by converting every character in location string to a numeric value and
        // accumulating every result
        private int GetHashValue(string location)
        {
            int hashValue = 0;
            for (int i = 0; i < location.Length; i++)
            {
                hashValue += Convert.ToInt32(location.ElementAt(i));
            }
            return hashValue;
        }
    }
}
