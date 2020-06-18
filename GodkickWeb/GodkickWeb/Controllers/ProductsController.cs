using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GodkickWeb.Models;
using System.Transactions;

namespace GodkickWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private CsK24T26Entities db = new CsK24T26Entities();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }
        [AllowAnonymous]
        public ActionResult Search(string keyword)
        {
            var model = db.Products.ToList();
            model = model.Where(p => p.name.ToLower().Contains(keyword.ToLower())).ToList();
            ViewBag.keyword = keyword;
            return View("List", model);
        }
        // for customer to view product
        [AllowAnonymous]
        public ActionResult List()
        {
            var model = db.Products.ToList();
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var model = db.Products.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.cate_id = new SelectList(db.Categories, "id", "name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product model, HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                if (picture != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        db.Products.Add(model);
                        db.SaveChanges();

                        // store picture
                        var path = Server.MapPath(Picture_PATH);
                        picture.SaveAs(path + model.id);

                        scope.Complete();
                        return RedirectToAction("List");
                    }

                }
                else ModelState.AddModelError("", "Picture not found!");
            }

            ViewBag.cate_id = new SelectList(db.Categories, "id", "name", model.cate_id);
            return View(model);
        }

        private const string Picture_PATH = "~/Upload/Products/";
        private void ValidateProduct(Product product)
        {
            if (product.price < 0)
                ModelState.AddModelError("price", "price is less than zero");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {

            var model = db.Products.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            ViewBag.cate_id = new SelectList(db.Categories, "id", "name", model.cate_id);
            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model, HttpPostedFileBase picture)
        {
            ValidateProduct(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();

                    if (picture != null)
                    {
                        var path = Server.MapPath(Picture_PATH);
                        picture.SaveAs(path + model.id);

                    }
                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.cate_id = new SelectList(db.Categories, "id", "name", model.cate_id);
            return View(model);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            var model = db.Products.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var scope = new TransactionScope())
            {
                var model = db.Products.Find(id);
                db.Products.Remove(model);
                db.SaveChanges();


                var path = Server.MapPath(Picture_PATH);
                System.IO.File.Delete(path + model.id);


                scope.Complete();
                return RedirectToAction("Index");
            }

        }

        [AllowAnonymous]
        public ActionResult Image(int id)
        {
            var path = Server.MapPath(Picture_PATH + id);
            //ViewBag.Test = id;
            //return File(path, "image");
            return File(path, "image");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AllowAnonymous]
        public ActionResult Picture(int id)
        {
            var path = Server.MapPath(Picture_PATH);
            return File(path + id, "images");
        }

    }
}

