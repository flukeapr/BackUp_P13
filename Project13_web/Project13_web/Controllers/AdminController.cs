using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project13_web.Models;

namespace Project13_web.Controllers
{
    public class AdminController : Controller
    {
        private DBEntities db = new DBEntities();
        CustomerIndexData customerIndexData = new CustomerIndexData();
        public ActionResult Main()
        {
            return View();
        }
        public ActionResult Order(int? bookId)
        {
            customerIndexData.books = db.Books.ToList();
            if (bookId != null)
            {

                customerIndexData.orders = from order in db.Orders
                                           where order.book_id == bookId
                                           select order;
            }
            var book = from p in db.Books
                       select p;



            return View(customerIndexData);
        }
        public ActionResult ReportAdmin()
        {


            return View(db.Orders.ToList());
        }
        public JsonResult GetReportJson()
        {
            var data = db.Orders.ToList();
            return Json(new { JSONList = data }, JsonRequestBehavior.AllowGet);
        }
        // GET: Admin
        public ViewResult Index(string Searchstring)
        {
            var book = from p in db.Books
                       select p;
            if (!String.IsNullOrEmpty(Searchstring))
            {
                book = book.Where(p => p.Book_Name.Contains(Searchstring) || p.Category.Contains(Searchstring));
            }
            return View(book.ToList());
        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            var TypeBookCat = new List<String>
                {
            "นิยาย" ,
            "การ์ตูน" ,
            "หนังสือเรียน" ,
            "หนังสือเด็ก"
            };

            ViewBag.selectedTypeBookCat = TypeBookCat;
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {

                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Book_Image"), fileName);
                    file.SaveAs(path);
                    book.Image = fileName;
                }

                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Book_Id,Book_Name,Category,Short_Story,Author,Publisher,Price,Image")] Book book)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Book_Image"), fileName);
                    file.SaveAs(path);
                    book.Image = fileName;
                }

                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
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
    }
}
