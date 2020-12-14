using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using doan_1.Models;
using Microsoft.AspNet.Identity;

namespace doan_1.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult ThongBao()
        {
            return View();
        }
        // GET: Orders
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Order.ToList());
        }
        
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserId,OrderDate,SubTotal,FullName,PhoneNumber,AddressDelivery,ThanhToan")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Order.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,UserId,OrderDate,SubTotal,FullName,PhoneNumber,AddressDelivery,ThanhToan")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Order.Find(id);
            db.Order.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //[Authorize(Roles = "Admin")]
        //public ActionResult ShowListOrderForAdmin()
        //{
        //    return View(db.Order.ToList());
        //}
        [Authorize(Roles = "User")]
        public ActionResult ShowListOrderForUser()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
            var danhSach = db.Order.Where(s => s.UserId == currentUserId);
            return View(danhSach.ToList());
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AddBill(int id, float hoadon, string tinhtrang)
        {
            if (tinhtrang == "Dang Giao")
            {
                var tim = db.Order.Where(s => s.OrderID == id);
                var user = db.Order.Find(id);
                if (tim != null)
                {
                    user.ThanhToan = "Thanh toan hoan tat";
                    Bill bil = new Bill();
                    bil.OrderID = id;
                    bil.IssueDate = DateTime.Now;
                    bil.TongHoaDon = hoadon;
                    db.Bills.Add(bil);
                    db.SaveChanges();
                }
                
            }
            else
            {
                return RedirectToAction("ThongBao");
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        public ActionResult DangGiao(int id, string tinhtrang)
        {
            if (tinhtrang == "Dang duyet")
            {
                var tim = db.Order.Where(s => s.OrderID == id);
                var user = db.Order.Find(id);
                //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                if (tim != null)
                {
                    user.OrderDate = DateTime.Now;
                    user.ThanhToan = "Dang Giao";
                    db.SaveChanges();
                }
            }
            else
            {
                return RedirectToAction("ThongBao");
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "User")]
        public ActionResult HuyDon(int id, string tinhtrang)
        {
            if (tinhtrang == "Dang duyet")
            {
                var tim = db.Order.Where(s => s.OrderID == id);
                var user = db.Order.Find(id);
                string currentUserId = User.Identity.GetUserId();
                //ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                if (tim != null)
                {
                    user.OrderDate = DateTime.Now;
                    user.ThanhToan = "Huy";
                    user.UserId = currentUserId;
                    db.SaveChanges();
                }
            
            }
            else
            {
                return RedirectToAction("ThongBao");
            }
            return RedirectToAction("ShowListOrderForUser");
        }
    }
}
