/* File name: GRDriverTest.cs
 * Description: Test class for driver table.
 *              It tests various validations for postal code.
 * Name: Gonzalo Ramos Zúñiga
 */

using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using GRBusService.Models;
using System.Data.Entity;
using GRBusService.Controllers;
using System.Web.Mvc;

namespace GRBusService.Tests.Controllers
{
    [TestFixture]
    public class GRDriverTest
    {
        driver driver;
        BusServiceContext db = new BusServiceContext();

        [SetUp]
        public void SetUp()
        {
            driver = new driver();
            driver.driverId = 0;
            driver.firstName = "First";
            driver.lastName = "Last";
            driver.fullName = "Last, First";
            driver.homePhone = "123-123-1234";
            driver.workPhone = "123-123-1234";
            driver.street = "123 Street";
            driver.city = "City";
            driver.postalCode = "A1A 1A1";
            driver.provinceCode = "ON";
            driver.dateHired = DateTime.Now;
        }

        [TearDown]
        public void TearDown()
        {
            db.Entry(driver).State = EntityState.Detached;
        }

        [Test]
        public void DriverPostalCodeUpperSpacePass()
        {
            driver.postalCode = "A1A 1A1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.Pass();
        }

        [Test]
        public void DriverPostalCodeUpperNoSpacePass()
        {
            driver.postalCode = "A1A1A1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.Pass();
        }

        [Test]
        public void DriverPostalCodeLowerSpacePass()
        {
            driver.postalCode = "a1a 1a1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.Pass();
        }

        [Test]
        public void DriverPostalCodeLowerNoSpacePass()
        {
            driver.postalCode = "a1a1a1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.Pass();
        }

        [Test]
        public void DriverPostalCodeInvalidFirstCharFail()
        {
            GRDriverController controller = new GRDriverController();
            driver.postalCode = "D1A 1A1";

            var result = controller.Create(driver);

            Assert.IsNotInstanceOf<ViewResult>(result, "");
        }

        [Test]
        public void DriverPostalCodeNoiseBeforeFail()
        {
            GRDriverController controller = new GRDriverController();
            driver.postalCode = "noiseA1A 1A1";

            var result = controller.Create(driver);

            Assert.IsNotInstanceOf<ViewResult>(result, "");
        }

        [Test]
        public void DriverPostalCodeNoiseAfterFail()
        {
            GRDriverController controller = new GRDriverController();
            driver.postalCode = "A1A 1A1noise";

            var result = controller.Create(driver);

            Assert.IsNotInstanceOf<ViewResult>(result, "");
        }

        [Test]
        public void DriverPostalCodeLowerShiftUpperPass()
        {
            driver.postalCode = "a1a 1a1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.AreEqual("A1A 1A1", driver.postalCode);
        }

        [Test]
        public void DriverPostalCodeNoSpaceAddSpacePass()
        {
            driver.postalCode = "A1A1A1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.AreEqual("A1A 1A1", driver.postalCode);
        }

        [Test]
        public void DriverPostalCodeSpaceAddNoOtherSpacePass()
        {
            driver.postalCode = "A1A 1A1";

            db.drivers.Add(driver);
            db.SaveChanges();

            Assert.AreEqual("A1A 1A1", driver.postalCode);
        }
    }
}