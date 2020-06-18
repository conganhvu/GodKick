using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GodkickWeb.Models;

namespace GodkickWeb.Controllers
{
    public class AspNetRoleUserController : Controller
    {
        CsK24T26Entities db = new CsK24T26Entities();
        // GET: AspNetRoleUser
        public ActionResult Create(string roleId)
        {
            ViewBag.Users = new SelectList(db.AspNetUsers.ToList(), "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string roleId, string userId)
        {
            if (ModelState.IsValid)
            {
                var user = db.AspNetUsers.Find(userId);
                var role = db.AspNetRoles.Find(roleId);
                role.AspNetUsers.Add(user);
                db.Entry(role).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "AspNetRole");
            }
            return RedirectToAction("Create");
        }
        // GET: AspNetRole/Delete/5
        public ActionResult Delete(string roleId, string userId)
        {
            var role = db.AspNetRoles.Find(roleId);
            var user = db.AspNetUsers.Find(userId);

            role.AspNetUsers.Remove(user);
            db.Entry(role).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index", "AspNetRoles");
        }



    }
}
