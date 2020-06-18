using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GodkickWeb.Models;

namespace GodkickWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        private CsK24T26Entities db = new CsK24T26Entities();

        private List<Bill_Info> ShoppingCart = null;

        public ShoppingCartController()
        {
            var session = System.Web.HttpContext.Current.Session;
            if (session["ShoppingCart"] != null)
                ShoppingCart = session["ShoppingCart"] as List<Bill_Info>;
            else
            {
                ShoppingCart = new List<Bill_Info>();
                session["ShoppingCart"] = ShoppingCart;
            }
        }

        // GET: ShoppingCart
        public ActionResult Index()
        {
            var hashtable = new Hashtable();
            foreach (var billDetail in ShoppingCart)
            {
                if (hashtable[billDetail.Product.id] != null)
                {
                    (hashtable[billDetail.Product.id] as Bill_Info).number += billDetail.number;
                }
                else hashtable[billDetail.Product.id] = billDetail;
            }

            ShoppingCart.Clear();
            foreach (Bill_Info billDetail in hashtable.Values)
                ShoppingCart.Add(billDetail);
            return View(ShoppingCart);
        }

        // GET: ShoppingCart/Create
        [HttpPost]
        public ActionResult Create(int? productId, int? quantity)
        {
            var product = db.Products.Find(productId);
            ShoppingCart.Add(new Bill_Info
            {
                Product = product,
                number = (byte)quantity
            });

            return RedirectToAction("Index");
        }

        // GET: ShoppingCart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Info bill_Info = db.Bill_Info.Find(id);
            if (bill_Info == null)
            {
                return HttpNotFound();
            }
            ViewBag.bill_id = new SelectList(db.Bills, "id", "id", bill_Info.bill_id);
            ViewBag.product_id = new SelectList(db.Products, "id", "name", bill_Info.product_id);
            return View(bill_Info);
        }


        // GET: ShoppingCart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Info bill_Info = db.Bill_Info.Find(id);
            if (bill_Info == null)
            {
                return HttpNotFound();
            }
            return View(bill_Info);
        }


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
