using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TinTucMoiNhat.Models;
using PagedList;
namespace TinTucMoiNhat.Controllers
{
    public class newsController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();
        public ActionResult Index(string keyword, int? pg)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (keyword == null) keyword = "";
            int pageSize = 35;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            int date_id = Config.datetimeidByDay(-1);
            if (keyword != "") date_id = Config.datetimeidByDay(-30);
            var p = (from q in db.news where q.date_id >= date_id && (q.name.Contains(keyword) || q.des.Contains(keyword)) select q).OrderByDescending(o => o.date_time).ToList();
            ViewBag.keyword = keyword;
            ViewBag.pg = pg;
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        // GET: news/Details/5
        public ActionResult Details(long? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        public ActionResult Post()
        {
            return View();
        }
        // GET: news/Create
        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: news/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,des,full_content,image,link,cat_id,keywords,date_time,time,date_id,pdf,status,loads")] news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                db.news.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: news/Edit/5
        public ActionResult Edit(long? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,des,full_content,image,link,cat_id,keywords,date_time,time,date_id,pdf,status,loads")] news news)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: news/Delete/5
        public ActionResult Delete(long? id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            news news = db.news.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: news/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            news news = db.news.Find(id);
            db.news.Remove(news);
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
        [HttpPost]
        public string hidepost(long id,int status)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update news set status=" + status + " where id=" + id);
                return "1";
            }
            catch
            {
                return "0";
            }
        }
    }
}
