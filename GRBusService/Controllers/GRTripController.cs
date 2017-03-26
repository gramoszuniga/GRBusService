/* File name: GRTripController.cs
 * Description: Controller for trip table.
 *              It creates and read records for trip table.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GRBusService.Models;
using GRBusService.Models.ViewModels;

namespace GRBusService.Controllers
{
    public class GRTripController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all trip records for a given route, sends list to Index view under GRTrip folder ordered by trip date and start time
        public ActionResult Index(string busRouteCode = "", string routeName = "")
        {
            if (busRouteCode != "")
            {
                Session["code"] = busRouteCode;
                Session["name"] = routeName;
            }
            else if (Session["code"] != null)
            {
                busRouteCode = (string)Session["code"];
                routeName = (string)Session["name"];
            }
            else
            {
                TempData["message"] = "Please select a bus route first to see its trips.";
                return RedirectToAction("Index", "GRBusRoute");
            }

            var trips = from t in db.trips
                        where (t.routeSchedule.busRouteCode == busRouteCode)
                        orderby t.tripDate descending, t.routeSchedule.startTime
                        select t;

            return View(trips.ToList());
        }


        // creates dropdown lists for route schedule id, driver id, bus id and calls Create view under GRTrip folder
        public ActionResult Create()
        {
            if (String.IsNullOrEmpty((string)Session["code"]))
            {
                TempData["message"] = "Please select a bus route first to create a trip for it.";
                return RedirectToAction("Index", "GRBusRoute");
            }
            string busRouteCode = (string)Session["code"];
            var routeSchedules = from rs in db.routeSchedules
                                 where (rs.busRouteCode == busRouteCode)
                                 orderby rs.isWeekDay descending, rs.startTime
                                 select rs;
            List<StartTimeTrip> startTimeTrips = new List<StartTimeTrip>();
            foreach (var item in routeSchedules)
            {
                startTimeTrips.Add(new StartTimeTrip { routeScheduleId = item.routeScheduleId.ToString(), startTime = item.startTime.ToString() + (item.isWeekDay ? " Weekday" : " Weekend") });
            }
            ViewBag.routeScheduleId = new SelectList(startTimeTrips, "routeScheduleId", "startTime");
            var drivers = from d in db.drivers
                          orderby d.fullName
                          select d;
            ViewBag.driverId = new SelectList(drivers, "driverId", "fullName");
            var buses = from b in db.buses
                        where (b.status.Equals("available"))
                        orderby b.busNumber
                        select b;
            ViewBag.busId = buses;
            return View();
        }

        // Captures a trip and if it is valid, creates new trip and calls Index view under GRTrip folder. Otherwise, creates dropdown
        // lists for route schedule id, driver id, bus id and calls Create view under GRTrip folder
        [HttpPost]
        public ActionResult Create(trip trip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.trips.Add(trip);
                    db.SaveChanges();
                    TempData["message"] = "Trip added.";
                    return RedirectToAction("Index");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError("", "Error: " + error.GetBaseException().Message);
                }
            }
            string busRouteCode = (string)Session["code"];
            var routeSchedules = from rs in db.routeSchedules
                                 where (rs.busRouteCode == busRouteCode)
                                 orderby rs.isWeekDay descending, rs.startTime
                                 select rs;
            List<StartTimeTrip> startTimeTrips = new List<StartTimeTrip>();
            foreach (var item in routeSchedules)
            {
                startTimeTrips.Add(new StartTimeTrip { routeScheduleId = item.routeScheduleId.ToString(), startTime = item.startTime.ToString() + (item.isWeekDay ? " Weekday" : " Weekend") });
            }
            ViewBag.routeScheduleId = new SelectList(startTimeTrips, "routeScheduleId", "startTime");
            var drivers = from d in db.drivers
                          orderby d.fullName
                          select d;
            ViewBag.driverId = new SelectList(drivers, "driverId", "fullName");
            var buses = from b in db.buses
                        where (b.status.Equals("available"))
                        orderby b.busNumber
                        select b;
            ViewBag.busId = buses;
            return View(trip);
        }
    }
}