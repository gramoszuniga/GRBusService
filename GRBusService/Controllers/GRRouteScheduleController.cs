/* File name: GRRouteScheduleController.cs
 * Description: Controller for routeSchedule table.
 *              It creates, read, updates and deletes records for routeSchedule table.
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
    public class GRRouteScheduleController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all route schedule records, sends list to Index view under GRRouteSchedule folder
        public ActionResult Index()
        {
            var routeSchedules = db.routeSchedules.Include(r => r.busRoute);
            return View(routeSchedules.ToList());
        }

        // if id is not null and matches a record key in routeSchedule table, sends route schedule found
        // to Details view under GRRouteSchedule folder
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // creates dropdown list for bus route code and calls Create view under GRRouteSchedule folder
        public ActionResult Create()
        {
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName");
            return View();
        }

        // captures route schedule id, bus route code, start time, if it is weekday or not and comments. If they are valid
        // creates dropdown list for bus route code, creates new route schedule and calls Index view under GRRouteSchedule folder.
        // Otherwise, calls Create view under GRRouteSchedule folder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.routeSchedules.Add(routeSchedule);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // if id is not null and matches a record key in routeSchedule table, creates dropdown list for bus route code and 
        // sends route schedule found to Edit view under GRRouteSchedule folder.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // captures route schedule id, bus route code, start time, if it is weekday or not and comments. If they are valid, modifies route schedule and 
        // calls Index view under GRRouteSchedule folder. Otherwise, calls Edit view under GRRouteSchedule folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "routeScheduleId,busRouteCode,startTime,isWeekDay,comments")] routeSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                db.Entry(routeSchedule).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.busRouteCode = new SelectList(db.busRoutes, "busRouteCode", "routeName", routeSchedule.busRouteCode);
            return View(routeSchedule);
        }

        // if id is not null and matches a record key in routeSchedule table, sends route schedule found
        // to Delete view under GRRouteSchedule folder
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            if (routeSchedule == null)
            {
                return HttpNotFound();
            }
            return View(routeSchedule);
        }

        // removes route schedule found from routeSchedule table and calls Index view under GRRouteSchedule folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            routeSchedule routeSchedule = db.routeSchedules.Find(id);
            db.routeSchedules.Remove(routeSchedule);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // if id is not null and matches a record key in routeStops table, lists schedule for a given bus route and bus stops.
        public ActionResult RouteStopSchedule(int routeStopId = 0)
        {
            if (routeStopId == 0)
            {
                TempData["message"] = "Please select a bus stop first to see its schedule.";
                return RedirectToAction("Index", "GRBusStop");
            }
            routeStop routeStop = db.routeStops.Find(routeStopId);
            if (routeStop == null)
            {
                TempData["message"] = "Sorry, there is route stop with that id.";
                return RedirectToAction("Index", "GRBusStop");
            }
            Session["busRoute"] = routeStop.busRouteCode + "-" + routeStop.busRoute.routeName;
            Session["busStop"] = Session["busStopNumber"] + "-" + Session["location"];
            var routeSchedules = db.routeSchedules.Where(rs => rs.busRouteCode.Equals(routeStop.busRouteCode)).OrderBy(rs => rs.startTime).ToList();
            if (routeSchedules.Count() == 0)
            {
                TempData["message"] = "There is no schedule for the selected route.";
                return RedirectToAction("Index", "GRBusStop");
            }
            foreach (var item in routeSchedules)
            {
                item.startTime += new TimeSpan(0, (int)routeStop.offsetMinutes, 0);
            }
            return View(routeSchedules);
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
