using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinTucMoiNhat.Models;
using PagedList;
namespace TinTucMoiNhat.Controllers
{
    public class postsController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();
        // GET: posts
        public ActionResult Index(string keyword, int? status, int? pg)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (Config.getCookie("user_id") != "1") return RedirectToAction("Login", "Account");
            if (status == null) status = 0;
            if (keyword == null) keyword = "";
            int pageSize = 35;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            int date_id = Config.datetimeidByDay(-1);
            if (keyword != "") date_id = Config.datetimeidByDay(-30);
            var p = (from q in db.posts where q.date_id >= date_id && q.status== status && (q.message.Contains(keyword) || q.id.ToString().Contains(keyword)) select q).OrderByDescending(o => o.date_post).ToList();
            ViewBag.keyword = keyword;
            ViewBag.pg = pg;
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public string hidepost(long id, int status)
        {
            if (Config.getCookie("logged") == "") return "0";
            if (Config.getCookie("user_id") != "1") return "0";
            try
            {
                db.Database.ExecuteSqlCommand("update posts set status=" + status + " where id=" + id);
                return "1";
            }
            catch
            {
                return "0";
            }
        }
        // GET: posts/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: posts/Create
        public ActionResult Create()
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (Config.getCookie("user_id") != "1") return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: posts/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: posts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: posts/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: posts/Delete/5
        public ActionResult Delete(int id)
        {
            if (Config.getCookie("logged") == "") return RedirectToAction("Login", "Account");
            if (Config.getCookie("user_id") != "1") return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: posts/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
