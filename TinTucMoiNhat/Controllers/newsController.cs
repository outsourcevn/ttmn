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
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.IO;
using System.Text;
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
        [HttpPost]
        public string Comment(string full_content, long id_news, string user_email, string user_name)
        {
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LegNS8UAAAAAMBmlZqhNUxfN3mG5_n31T5azTcn";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            ViewBag.Message = status ? "Gửi bình luận thành công <a href='#' onclick='javascript:window.history.go(-1);'>Quay lại</a>" : "Gửi không thành công do server phát hiện Bạn chưa click để xác nhận không phải là robot gửi tự động <a href='#' onclick='javascript:window.history.go(-1);'>Quay lại</a>!";
            if (status)
            {
                comment bl = new comment();
                bl.date_id = Config.datetimeid();
                bl.date_time = DateTime.Now;
                bl.full_content = full_content;
                bl.news_id = id_news;
                bl.status = 0;
                bl.user_email = user_email;
                bl.user_name = user_name;
                if (Config.getCookie("user_id") != "")
                {
                    bl.user_id = long.Parse(Config.getCookie("user_id"));
                }
                db.comments.Add(bl);
                db.SaveChanges();
                Notification(0, id_news, user_name,full_content);
            }
            else
            {

            }
            return ViewBag.Message;
        }
        [HttpPost]
        public string Comment2(string full_content, long id_news, string user_email, string user_name)
        {
            var response = Request["g-recaptcha-response"];
            string secretKey = "6LegNS8UAAAAAMBmlZqhNUxfN3mG5_n31T5azTcn";
            var client = new WebClient();
            var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));
            var obj = JObject.Parse(result);
            var status = (bool)obj.SelectToken("success");
            ViewBag.Message = status ? "Gửi bình luận thành công <a href='#' onclick='javascript:window.history.go(-1);'>Quay lại</a>" : "Gửi không thành công do server phát hiện Bạn chưa click để xác nhận không phải là robot gửi tự động <a href='#' onclick='javascript:window.history.go(-1);'>Quay lại</a>!";
            if (status)
            {
                comment bl = new comment();
                bl.date_id = Config.datetimeid();
                bl.date_time = DateTime.Now;
                bl.full_content = full_content;
                bl.post_id = id_news;
                bl.status = 0;
                bl.user_email = user_email;
                bl.user_name = user_name;
                if (Config.getCookie("user_id") != "")
                {
                    bl.user_id = long.Parse(Config.getCookie("user_id"));
                }
                db.comments.Add(bl);
                db.SaveChanges();
                Notification(1, id_news, user_name, full_content);
            }
            else
            {

            }
            return ViewBag.Message;
        }
        public string Notification(int type,long id,string user_name,string full_content)
        {
            try
            {
               
                string link = "http://tintucmoinhat.vn";
                string sheadings = user_name;
                string push_content = full_content;
                if (type == 0)
                {
                    var ne = db.news.Find(id);
                    link = "http://tintucmoinhat.vn/" + Config.getCatName(ne.cat_id) + "/" + Config.unicodeToNoMark(ne.name) + "-" + ne.id;
                    push_content = full_content;// + " " + link
                    sheadings = user_name;

                }
                else
                {
                    var ne = db.posts.Find(id);
                    link = "http://tintucmoinhat.vn/fanpost/bai-viet/" + Config.unicodeToNoMark(ne.domain_name) + "-" + ne.id;
                    push_content = full_content;
                    sheadings = user_name;
                }
                var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
                request.KeepAlive = true;
                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";

                request.Headers.Add("authorization", "Basic MjI5ODQ5MzgtN2M2OC00MGVhLWIzNTgtNmMzYjA3M2VkZDhj");

                var serializer = new JavaScriptSerializer();
                var obj = new
                {
                    app_id = "d33eb61e-6b59-4087-a47e-76c762641c3b",
                    contents = new { en = push_content },
                    url = link,
                    headings = new { en = sheadings },
                    included_segments = new string[] { "All" }
                };// headings=sheadings,
                var param = serializer.Serialize(obj);
                byte[] byteArray = Encoding.UTF8.GetBytes(param);

                string responseContent = null;

                try
                {
                    using (var writer = request.GetRequestStream())
                    {
                        writer.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = request.GetResponse() as HttpWebResponse)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                        }
                    }
                }
                catch (WebException ex)
                {
                    return ex.Message;

                }
                return responseContent;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
