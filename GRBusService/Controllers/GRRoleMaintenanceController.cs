/* File name: GRRoleMaintenanceController.cs
 * Description: Controller for AspNetRoles table.
 *              It creates, reads, updates and deletes records for AspNetRoles table.
 * Name: Gonzalo Ramos Zúñiga
 */

using GRBusService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GRBusService.Controllers
{
    [Authorize(Roles = "administrators")]
    public class GRRoleMaintenanceController : Controller
    {
        private static ApplicationDbContext db = GRUserMaintenanceController.db;
        private UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        IdentityResult identityResult;

        // lists all AspNetRoles records, sends list to Index view under GRRoleMaintenance folder order by name
        public ActionResult Index()
        {
            return View(roleManager.Roles.OrderBy(r => r.Name));
        }

        // if role does not already exit, creates new role and calls Create view under GRRoleMaintenance folder
        public ActionResult Create(string Role)
        {
            if (Role != null && Role.Trim() != "")
            {
                IdentityRole role = roleManager.FindByName(Role);
                if (role == null)
                {
                    try
                    {
                        Role = Role.Trim();
                        identityResult = roleManager.Create(new IdentityRole(Role));
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "Role added.";
                        }
                        else
                        {
                            TempData["message"] = "Add role failed: " + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Index");
                }
                TempData["message"] = "The role already exit.";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Please input a role before creating.";
            return RedirectToAction("Index");
        }

        // If role found has no members, deletes it. Otherwise, finds all its members and sends them to the Delete view under GRRoleMaintenance folder
        public ActionResult Delete(string roleId)
        {
            IdentityRole role = roleManager.FindById(roleId);
            if (role != null)
            {
                if (role.Name != "administrators")
                {
                    if (role.Users.Count() == 0)
                    {
                        try
                        {
                            identityResult = roleManager.Delete(role);
                            if (identityResult.Succeeded)
                            {
                                TempData["message"] = role.Name + " role deleted.";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["message"] = "Delete role failed: " + identityResult.Errors.ElementAt(0);
                            }
                        }
                        catch (Exception error)
                        {
                            TempData["message"] = "Error: " + error.GetBaseException().Message;
                        }
                        return RedirectToAction("Delete", new { roleId = role.Id });
                    }
                    List<ApplicationUser> users = new List<ApplicationUser>();
                    foreach (var item in role.Users)
                    {
                        users.Add(userManager.FindById(item.UserId));
                    }
                    Session["deleteRoleId"] = role.Id;
                    return View(users.OrderBy(u => u.UserName));
                }
                TempData["message"] = "The administrators role cannot be deleted.";
                return RedirectToAction("Index");
            }
            TempData["message"] = "Please select a role first.";
            return RedirectToAction("Index");
        }

        // removes role found from AspNetRoles table and calls Index view under GRRoleMaintenance folder
        public ActionResult DeleteConfirmed(string id)
        {
            IdentityRole role = roleManager.FindById(Session["deleteRoleId"].ToString());
            if (role != null)
            {
                try
                {
                    identityResult = roleManager.Delete(role);
                    if (identityResult.Succeeded)
                    {
                        TempData["message"] = "Role deleted.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Delete role failed: " + identityResult.Errors.ElementAt(0);
                    }
                }
                catch (Exception error)
                {
                    TempData["message"] = "Error: " + error.GetBaseException().Message;
                }
                return RedirectToAction("Delete", new { roleId = role.Id });
            }
            TempData["message"] = "Please select a role first.";
            return RedirectToAction("Index");
        }

        // finds all users members of the given role and sends them to the Manage view under GRRoleMaintenance folder
        public ActionResult Manage(string roleId)
        {
            IdentityRole role = roleManager.FindById(roleId);
            if (role != null)
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                foreach (var item in role.Users)
                {
                    users.Add(userManager.FindById(item.UserId));
                }
                ViewBag.UserId = new SelectList(userManager.Users.ToList().Except(users), "Id", "UserName");
                Session["roleId"] = roleId;
                return View(users.OrderBy(u => u.UserName));
            }
            TempData["message"] = "Please select a role first.";
            return RedirectToAction("Index");
        }

        // removes found user from the corresponding role
        public ActionResult Remove(string userId)
        {
            if (Session["roleId"] != null)
            {
                string roleId = Session["roleId"].ToString();
                ApplicationUser user = userManager.FindById(userId);
                if (user != null)
                {
                    IdentityRole role = roleManager.FindById(Session["roleId"].ToString());
                    try
                    {
                        identityResult = userManager.RemoveFromRole(userId, role.Name);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "User removed from role.";
                        }
                        else
                        {
                            TempData["message"] = "Removing user from role failed: " + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Manage", new { roleId = role.Id });
                }
                TempData["message"] = "Please select a role first before removing a user.";
                return RedirectToAction("Manage", new { roleId = roleId });
            }
            TempData["message"] = "Please select a role first.";
            return RedirectToActionPermanent("Index");
        }

        // adds found user to the corresponding role
        public ActionResult Add(string UserId)
        {
            if (Session["roleId"] != null)
            {
                string roleId = Session["roleId"].ToString();
                ApplicationUser user = userManager.FindById(UserId);
                if (user != null)
                {
                    IdentityRole role = roleManager.FindById(Session["roleId"].ToString());
                    try
                    {
                        identityResult = userManager.AddToRole(UserId, role.Name);
                        if (identityResult.Succeeded)
                        {
                            TempData["message"] = "User added to role.";
                        }
                        else
                        {
                            TempData["message"] = "Adding user to role failed: " + identityResult.Errors.ElementAt(0);
                        }
                    }
                    catch (Exception error)
                    {
                        TempData["message"] = "Error: " + error.GetBaseException().Message;
                    }
                    return RedirectToAction("Manage", new { roleId = role.Id });
                }
                TempData["message"] = "Please select a role first before adding a user.";
                return RedirectToAction("Manage", new { roleId = roleId });
            }
            TempData["message"] = "Please select a role first.";
            return RedirectToActionPermanent("Index");
        }
    }
}