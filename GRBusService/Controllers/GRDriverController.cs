/* File name: GRDriverController.cs
 * Description: Controller for driver table.
 *              It creates, reads, updates and deletes records for driver table.
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.App_GlobalResources;
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
    public class GRDriverController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all driver records, sends list to Index view under GRDriver folder order by full name
        public ActionResult Index()
        {
            var drivers = db.drivers.Include(d => d.province).OrderBy(d => d.fullName);
            return View(drivers.ToList());
        }

        // if id is not null and matches a record key in driver table, sends driver found
        // to Details view under GRDriver folder
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // creates dropdown list for province code ordered by name and calls Create view under GRDriver folder
        public ActionResult Create()
        {
            ViewBag.provinceCode = new SelectList(db.provinces.OrderBy(p => p.name), "provinceCode", "name");
            return View();
        }

        // captures driver id, first name, last name, home phone, work phone, street, city, postal code, province code and date hired. 
        // If they are valid creates new driver and calls Index view under GRDriver folder. Otherwise, creates dropdown list ordered by
        // name for province code and calls Create view under GRDriver folder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "driverId,firstName,lastName,homePhone,workPhone,street,city,postalCode,provinceCode,dateHired")] driver driver)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.drivers.Add(driver);
                    db.SaveChanges();
                    TempData["message"] = GRTranslations.driverAdded;
                    return RedirectToAction("Index");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError("", GRTranslations.exceptionOnAdd + error.GetBaseException().Message);
                }
            }

            ViewBag.provinceCode = new SelectList(db.provinces.OrderBy(p => p.name), "provinceCode", "name", driver.provinceCode);
            return View(driver);
        }

        // if id is not null and matches a record key in driver table, sends driver found to Edit view under 
        // GRDriver folder. Otherwise, creates dropdown list for province code ordered by name and calls Edit view under GRDriver folder.
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            ViewBag.provinceCode = new SelectList(db.provinces.OrderBy(p => p.name), "provinceCode", "name", driver.provinceCode);
            Session["fullName"] = driver.fullName;
            return View(driver);
        }

        // captures driver id, first name, last name, home phone, work phone, street, city, postal code, province code and date hired. 
        // If they are valid, updates driver and calls Index view under GRDriver folder. Otherwise, creates dropdown list for province
        // code ordered by name and calls Edit view under GRDriver folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "driverId,firstName,lastName,homePhone,workPhone,street,city,postalCode,provinceCode,dateHired")] driver driver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(driver).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = GRTranslations.driverEdited;
                    return RedirectToAction("Index");
                }
                catch (Exception error)
                {
                    ModelState.AddModelError("", GRTranslations.exceptionOnEdit + error.GetBaseException().Message);
                }
            }
            ViewBag.provinceCode = new SelectList(db.provinces.OrderBy(p => p.name), "provinceCode", "name", driver.provinceCode);
            return View(driver);
        }

        // if id is not null and matches a record key in driver table, sends driver found
        // to Delete view under GRDriver folder
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            driver driver = db.drivers.Find(id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        // removes driver found from driver table and calls Index view under GRDriver folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                driver driver = db.drivers.Find(id);
                db.drivers.Remove(driver);
                db.SaveChanges();
                TempData["message"] = GRTranslations.driverDeleted;
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["message"] = GRTranslations.exceptionOnDelete + error.GetBaseException().Message;
                return RedirectToAction("Delete", new { id = id });
            }
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
