using System.Web;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GodkickWeb.Controllers;
using GodkickWeb.Models;
using System.Linq;
using Moq;
using System.Transactions;

namespace GodkickWeb.Tests.Controllers
{
    [TestClass]
    public class ProductsControllerTest
    {
        [TestMethod]
        public void TestIndex()
        {
            var controller = new ProductsController();

            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<Product>;
            Assert.IsNotNull(model);

            var db = new CsK24T26Entities();
            Assert.AreEqual(db.Products.Count(), model.Count);
        }

        [TestMethod]
        public void TestList()
        {
            var controller = new ProductsController();

            var result = controller.List() as ViewResult;
            Assert.IsNotNull(result);

            var model = result.Model as List<Product>;
            Assert.IsNotNull(model);

            var db = new CsK24T26Entities();
            Assert.AreEqual(db.Products.Count(), model.Count);
        }

        [TestMethod]
        public void TestCreateG()
        {
            var controller = new ProductsController();

            var result = controller.Create() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestCreateP()
        {
            var rand = new Random();
            var product = new Product
            {
                name = rand.NextDouble().ToString(),
                product_description = rand.NextDouble().ToString(),
                price = -rand.Next()
            };

            var controller = new ProductsController();

            var result0 = controller.Create(product, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("price is less than zero", controller.ModelState["price"].Errors[0].ErrorMessage);

            product.price = -product.price;
            controller.ModelState.Clear();

            result0 = controller.Create(product, null) as ViewResult;
            Assert.IsNotNull(result0);
            Assert.AreEqual("Picture not found!", controller.ModelState[""].Errors[0].ErrorMessage);

            var picture = new Mock<HttpPostedFileBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server).Returns(server.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);

            var filename = String.Empty;
            server.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(s => s);
            picture.Setup(P => P.SaveAs(It.IsAny<string>())).Callback<string>(s => filename = s);
           
            using (var scope = new TransactionScope())
            {
                controller.ModelState.Clear();
                var result1 = controller.Create(product, picture.Object) as RedirectToRouteResult;
                Assert.IsNotNull(result1);
                Assert.AreEqual("Index", result1.RouteValues["action"]);


                var db = new CsK24T26Entities();
                var entity = db.Products.SingleOrDefault(p => p.name == product.name && p.product_description == product.product_description);
                Assert.IsNotNull(entity);
                Assert.AreEqual(product.price, entity.price);

                Assert.IsTrue(filename.StartsWith("~/Upload/products/"));
                Assert.IsTrue(filename.EndsWith(entity.id.ToString()));     
            }
        }
        
    }
}
