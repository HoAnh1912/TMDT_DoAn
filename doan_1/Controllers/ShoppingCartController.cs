using System.Data.Common;
using doan_1.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
namespace doan_1.Controllers
{
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext _db = new ApplicationDbContext();
        // GET: ShoppingCart
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddtoCart(int id)
        {
            var pro = _db.Book.SingleOrDefault(s => s.BookID == id);
            if (pro != null)
            {
                GetCart().Add(pro);
            }
            return Redirect(Request.UrlReferrer.ToString());

        }
        public ActionResult AddtoCart_Detail(int id)
        {
            var pro = _db.Book.SingleOrDefault(s => s.BookID == id);
            if (pro != null)
            {
                GetCart().Add(pro);
            }
              return RedirectToAction("ShowToCart", "ShoppingCart");

        }


        public ActionResult ShowToCart()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            return View(cart);
        }

           public ActionResult Update_Quantity_Cart(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            int id_pro = Convert.ToInt32(form["ID_Product"]);
            int _quantity = int.Parse(form["Quantity"]);
            cart.Update_Quantity_Shopping(id_pro, _quantity);
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }
        public ActionResult RemoveCart(int id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_Cart_Item(id);
            return Redirect(Request.UrlReferrer.ToString());
        }
        public PartialViewResult BagCart()
        {
            int total_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if (cart != null)
                total_item = cart.Total_Quantity_In_Cart();
            ViewBag.QuantityCart = total_item;

            return PartialView("BagCart");

        }
        public PartialViewResult BagCartItem()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return PartialView();
            }
            Cart cart = Session["Cart"] as Cart;
            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return PartialView();
            }
            return PartialView(cart);

        }
        public ActionResult ThanhToan()
        {
            if (Session["Cart"] == null)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
            Cart cart = Session["Cart"] as Cart;
            if (cart.Items.Count() == 0)
            {
                ViewBag.nullCart = "Giỏ hàng trống...!";
                return View();
            }
           
            return View(cart);
        }
        [HttpPost]
        public ActionResult CheckOut(string email, string diachi, string sdt)
        {
            try
            {
                float total = 0;
                Cart cart = Session["Cart"] as Cart;
                Order _order = new Order();
                _order.OrderDate = DateTime.Now;
                _order.ThanhToan = "Dang duyet";
                _order.FullName = email;
                _order.AddressDelivery = diachi;
                _order.PhoneNumber = sdt;
                _order.UserId = User.Identity.GetUserId();
                
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(x => x.Id == currentUserId);
                foreach (var item in cart.Items)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderID = _order.OrderID;
                    orderDetail.BookID = item._shopping_product.BookID;
                    orderDetail.UnitPriceSale = item._shopping_product.BookPrice;
                    orderDetail.Quantity = item._shopping_quantity;
                    total += item._shopping_quantity * item._shopping_product.BookPrice;
                    _db.OrderDetail.Add(orderDetail);

                }
                string content = System.IO.File.ReadAllText(Server.MapPath("~/Template/SendMailOrder.html"));
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
                if (currentUser != null)
                {
                    content = content.Replace("{{OrderDate}}", _order.OrderDate.ToString());
                    content = content.Replace("{{CustomerName}}", currentUser.UserName);
                    content = content.Replace("{{Phone}}", currentUser.PhoneNumber);
                    content = content.Replace("{{Email}}", currentUser.Email);
                    content = content.Replace("{{Address}}", currentUser.Address);
                    content = content.Replace("{{Total}}", total.ToString("N0"));
                    new MailHelper().SendMail(currentUser.Email, "Đơn hàng mới từ Shop Ba Chị Em", content);
                    new MailHelper().SendMail(toEmail, "Đơn hàng mới từ Shop Ba Chị Em", content);
                }
                else
                {
                    content = content.Replace("{{OrderDate}}", _order.OrderDate.ToString());
                    content = content.Replace("{{Total}}", total.ToString("N0"));
                    content = content.Replace("{{Phone}}", sdt);
                    content = content.Replace("{{Email}}", email);
                    content = content.Replace("{{Address}}", diachi);
                    new MailHelper().SendMail(email, "Đơn hàng mới từ Shop Ba Chị Em", content);
                    new MailHelper().SendMail(toEmail, "Đơn hàng mới từ Shop Ba Chị Em", content);
                }
                //
                
                _order.SubTotal = total;
                _db.Order.Add(_order);
                _db.SaveChanges();
                cart.ClearCart();
                return View("ThanhCong");
            }
            catch
            {
                return Content("Error CHeckout. Please information of Customer....");
            }
        }
    }
}