/* File name: GRUserMaintenanceController.cs
 * Description: Controller for AspNetUsers table.
 *              It reads, updates and deletes records for AspNetUsers table.
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GRBusService.Controllers
{
    [Authorize(Roles = "administrators")]
    public class GRUserMaintenanceController : Controller
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        IdentityResult identityResult;

        // lists all AspNetUsers records, sends list to Index view under GRUserMaintenance folder order by username
        public ActionResult Index()
        {
            return View(userManager.Users.OrderByDescending(u => u.LockoutEnabled).ThenBy(u => u.UserName).ToList());
        }

        // removes user found from all roles and from AspNetUsers table and calls Index view under GRUserMaintenance folder
        public ActionResult Delete(string userId)
        {
            ApplicationUser user = userManager.FindById(userId);
            if (user != null)
            {
                if (!userManager.IsInRole(userId, "administrators"))
                {
                    try
                    {
                        identityResult = userManager.Delete(user);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "User deleted.";
                        }
                        else
                        {
                            TempData["message"] = "Delete failed: " + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Administrators cannot be deleted.";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Please select a user first.";
            return RedirectToAction("Index");
        }

        // if current user is in the administrators role, sends user found to Reset view under GRUserMaintenance folder
        public ActionResult Reset(string userId)
        {
            if (User.IsInRole("administrators"))
            {
                ApplicationUser user = userManager.FindById(userId);
                if (user != null)
                {
                    if (!userManager.IsInRole(userId, "administrators"))
                    {
                        return View(user);
                    }
                    TempData["message"] = "The password of an administrator cannot be changed.";
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Please select a user first.";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Only members of the administrators role can reset passwords.";
            return RedirectToAction("Index");
        }

        // if passwords match, resets found user's password and calls Index view under GRUserMaintenance folder
        public ActionResult ResetConfirmed(string UserName, string newPassword, string confirmedPassword)
        {
            ApplicationUser user = userManager.FindByName(UserName);
            if (user != null)
            {
                if (newPassword == confirmedPassword)
                {
                    try
                    {
                        var provider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("GRBusService");
                        userManager.UserTokenProvider = new Microsoft.AspNet.Identity.Owin.DataProtectorTokenProvider<ApplicationUser>(provider.Create("PasswordReset"));
                        string passwordToken = userManager.GeneratePasswordResetToken(user.Id);
                        identityResult = userManager.ResetPassword(user.Id, passwordToken, newPassword);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "Password reset.";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            TempData["message"] = "Password reset failed." + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Reset", new { userId = user.Id });
                }
                TempData["message"] = "Passwords did not match.";
                return RedirectToAction("Reset", new { userId = user.Id });
            }
            TempData["message"] = "Please select a user first.";
            return RedirectToAction("Index");
        }

        // Unlocks found user if locked and locks it if unlocked
        public ActionResult LockUnlock(string userId)
        {
            ApplicationUser user = userManager.FindById(userId);
            if (user != null)
            {
                if (!userManager.IsInRole(user.Id, "administrators"))
                {
                    try
                    {
                        user.LockoutEnabled = !user.LockoutEnabled;
                        user.LockoutEndDateUtc = null;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        if (user.LockoutEnabled)
                        {
                            TempData["message"] = user.UserName + " has been locked.";
                        }
                        else
                        {
                            TempData["message"] = user.UserName + " has been unlocked.";
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Index");
                }
                TempData["message"] = "Administrators cannot be locked.";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Please select a user first.";
            return RedirectToAction("Index");
        }
    }
}