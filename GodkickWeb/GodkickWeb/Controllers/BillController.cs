using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GodkickWeb.Models;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using System.Text;

namespace GodkickWeb.Controllers
{
    public class BillController : Controller
    {
        private CsK24T26Entities db = new CsK24T26Entities();

        private List<Bill_Info> ShoppingCart = null;

        private void GetShoppingCart()
        {
            if (Session["ShoppingCart"] != null)
                ShoppingCart = Session["ShoppingCart"] as List<Bill_Info>;
            else
            {
                ShoppingCart = new List<Bill_Info>();
                Session["ShoppingCart"] = ShoppingCart;
            }
        }
        // GET: Bill
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = db.Bills.ToList();
            return View(model);
        }
        public ActionResult Create()
        {
            GetShoppingCart();
            ViewBag.Cart = ShoppingCart;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bill model, string email, string phone)
        {
            ValidateBill(model, email, phone);
            CheckLoggedIn(User);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    model.order_day = DateTime.Now;
                    model.user_id = User.Identity.GetUserId();
                    db.Bills.Add(model);
                    db.SaveChanges();
                    foreach (var item in ShoppingCart)
                    {
                        db.Bill_Info.Add(new Bill_Info
                        {
                            bill_id = model.id,
                            product_id = item.Product.id,
                            price = item.Product.price,
                            number = item.number
                        });
                    }
                    db.SaveChanges();

                    scope.Complete();
                    Session["ShoppingCart"] = null;
                    return RedirectToAction("List", "Products");
                }
            }
            GetShoppingCart();
            TempData["MessageError"] = "You haven't login yet or bill invalid . Please check again .";
            ViewBag.Cart = ShoppingCart;
            //ViewBag.MessageError = "You haven't login yet or bill invalid . Please check again .";
            return RedirectToAction("Index", "ShoppingCart");
        }

        private void CheckLoggedIn(IPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
                ModelState.AddModelError("User", "User must login first");
        }

        private void ValidateBill(Bill model, string email, string phone)
        {
            var regex = new Regex("^0\\d{9,10}$");
            GetShoppingCart();
            if (ShoppingCart.Count == 0)
                ModelState.AddModelError("", "There is no item in shopping cart!");
            if (!regex.IsMatch(phone))
                ModelState.AddModelError("phone", "Not a valid phonenumber");
            regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$");
            if (!regex.IsMatch(email))
                ModelState.AddModelError("email", "Not a valid email address");
        }

        public ActionResult search(string keyword)
        {
            var model = db.Bills.ToList();
            if (int.TryParse(keyword, out int id))
                model = model.Where(p => p.id == id).ToList();
            ViewBag.keyword = keyword;
            return View("Index", model);
        }
    }
}