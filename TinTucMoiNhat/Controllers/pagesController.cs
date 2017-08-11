using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TinTucMoiNhat.Models;

namespace TinTucMoiNhat.Controllers
{
    public class pagesController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();

        // GET: pages
        //[Authorize]
        public ActionResult Index()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            return View(db.pages.OrderByDescending(o=>o.id).ToList());
        }
        public ActionResult Login(string pass)
        {
            if (pass == "abc123")
            {
                Config.setCookie("logged", "1");
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Login","Account");
            }
        }
        public ActionResult LogOff()
        {
            Config.removeCookie("logged");
            return RedirectToAction("Login", "Account");
        }
        // GET: pages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            page page = db.pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }

        // GET: pages/Create
        //[Authorize]
        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: pages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,domain,name,page_id,cat_id,status")] page page)
        {
            if (ModelState.IsValid)
            {
                db.pages.Add(page);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(page);
        }

        // GET: pages/Edit/5
        //[Authorize]
        public ActionResult Edit(int? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            page page = db.pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }

        // POST: pages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,domain,name,page_id,cat_id,status")] page page)
        {
            if (ModelState.IsValid)
            {
                db.Entry(page).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(page);
        }

        // GET: pages/Delete/5
        //[Authorize]
        public ActionResult Delete(int? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            page page = db.pages.Find(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            return View(page);
        }

        // POST: pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            page page = db.pages.Find(id);
            db.pages.Remove(page);
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
