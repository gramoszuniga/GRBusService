/* File name: GRCountryController.cs
 * Description: Controller for country table.
 *              It creates, read, updates and deletes records for country table.
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
    public class GRCountryController : Controller
    {
        private BusServiceContext db = new BusServiceContext();

        // lists all country records, sends list to Index view under GRCountry folder
        public ActionResult Index()
        {
            return View(db.countries.ToList());
        }

        // if id is not null and matches a record key in country table, sends country found
        // to Details view under GRCountry folder
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // calls Create view under GRCountry folder
        public ActionResult Create()
        {
            return View();
        }

        // captures country code, name, postal pattern and phone pattern. If they are valid 
        // creates new country and calls Index view under GRCountry folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "countryCode,name,postalPattern,phonePattern")] country country)
        {
            if (ModelState.IsValid)
            {
                db.countries.Add(country);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(country);
        }

        // if id is not null and matches a record key in country table, sends country found
        // to Edit view under GRCountry folder. Otherwise, calls Edit view under GRCountry folder.
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // captures country code, name, postal pattern and phone pattern. If they are valid 
        // updates country and calls Index view under GRCountry folder. Otherwise, calls 
        // Edit view under GRCountry folder.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "countryCode,name,postalPattern,phonePattern")] country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        // if id is not null and matches a record key in country table, sends country found
        // to Delete view under GRCountry folder
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            country country = db.countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // removes country found from country table and calls Index view under GRCountry folder
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            country country = db.countries.Find(id);
            db.countries.Remove(country);
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
