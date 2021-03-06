﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using doan_1.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace doan_1.Controllers
{
    public class BookController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Book
       
        public ActionResult Index()
        {
            var book = db.Book.Include(b => b.Author).Include(b => b.Category).Include(b => b.Provider).Include(b => b.Publisher);
            return View(book.ToList());
        }

        // GET: Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            
            var chon = db.Comment.Where(s => s.BookID == id ).ToList();
            ViewData["List"] = chon;
            return View(book);
           
        }

        [Authorize(Roles = "Admin")]
        // GET: Book/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName");
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName");
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName");
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Book book)
        {
            if (ModelState.IsValid)
            {
                if (book.ImageUpLoad != null)
                {
                    string fileNameImg = Path.GetFileNameWithoutExtension(book.ImageUpLoad.FileName);
                    string extension = Path.GetExtension(book.ImageUpLoad.FileName);
                    fileNameImg = fileNameImg + extension;
                    book.Image = "~/Content/Images/" + fileNameImg;
                    book.ImageUpLoad.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/"), fileNameImg));
                }
                db.Book.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }
        [Authorize(Roles = "Admin")]
        // GET: Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,BookName,BookPrice,BookDescription,PublisherDate,Image,AuthorID,PublisherID,ProviderID,CateID")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Author, "AuthorID", "AuthorName", book.AuthorID);
            ViewBag.CateID = new SelectList(db.Category, "CateID", "CateName", book.CateID);
            ViewBag.ProviderID = new SelectList(db.Provider, "ProviderID", "ProviderName", book.ProviderID);
            ViewBag.PublisherID = new SelectList(db.Publisher, "PublisherID", "PublisherName", book.PublisherID);
            return View(book);
        }
   
        // GET: Book/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Book.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Book.Find(id);
            db.Book.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        
        public ActionResult LoadSachTheoDanhMuc(string name)
        {
            var ten_loai = name;
            var tenDanhMuc = db.Book.Where(m => m.Category.CateName == ten_loai);
            return View("Index",tenDanhMuc.ToList());
           
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult SearchTenSach(string tenSach)
        {
            var tenSachs = from m in db.Book select m;

            if (!String.IsNullOrEmpty(tenSach))
            {
                tenSachs = tenSachs.Where(s => s.BookName.Contains(tenSach)|| s.BookPrice.ToString()==tenSach||s.Category.CateName==tenSach);
            }
            return View("Index", tenSachs);

        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public ActionResult PosstComment(string bL, int id)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
           
            Comment newPost = new Comment();
            newPost.UserId = currentUserId;
            newPost.NoiDungBL = bL;
            newPost.BookID = id;
            newPost.Image = currentUser.Image;
            newPost.UserName = currentUser.UserName;
            db.Comment.Add(newPost);
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.PathAndQuery);
        }
    }
}
