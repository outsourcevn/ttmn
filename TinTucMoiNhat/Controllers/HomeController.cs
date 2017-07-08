using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using ReadSharp;
using System.Threading.Tasks;
using TinTucMoiNhat.Models;
using System.Text.RegularExpressions;
using System.IO;
using SelectPdf;
using PagedList;
using System.Text;
namespace TinTucMoiNhat.Controllers
{
    public class HomeController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();
        public ActionResult Index(string catname, int? cat_id, int? pg)
        {
            if (cat_id == null) cat_id = 0;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            int date_id = Config.datetimeidByDay(-10);
            var p = (from q in db.news where q.cat_id == cat_id && q.date_id >= date_id select q).OrderByDescending(o => o.id).ToList();
            var p2 = (from q2 in db.news where q2.date_id >= date_id select q2).OrderByDescending(o => o.date_time).Take(15).ToList();
            ViewBag.news = p2;
            var p3 = (from q3 in db.news where q3.cat_id == 0 && q3.date_id >= date_id select q3).OrderByDescending(o => o.loads).ThenByDescending(o => o.date_time).Take(10).ToList();
            ViewBag.ppl = p3;
            ViewBag.catname = Config.getCatName(cat_id);
            ViewBag.catfullname = Config.getFullCatName(cat_id);
            ViewBag.cat_id = cat_id;
            return View(p.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Detail(int id)
        {
            var p = db.news.Find(id);
            try {
                var p2 = (from q2 in db.news where q2.id >= id - 10 && q2.id <= id + 10 select q2).OrderByDescending(o => o.date_time).Take(15).ToList();
                ViewBag.news = p2;
                ViewBag.des = Regex.Replace(p.des, "<.*?>", string.Empty);
                db.Database.ExecuteSqlCommand("update news set loads=loads+1 where id=" + id);
                return View(p);
            }
            catch
            {
                return HttpNotFound();
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        //public string getAllContent(string link,ref string image)
        //{
        //    try
        //    {
        //        Reader reader = new Reader();
        //        Article result = await reader.Read(new Uri(link));
        //        if (image == "")
        //        {
        //            image = result.FrontImage.AbsolutePath;
        //        }
        //        return result.Content;
        //    }catch(Exception ex){
        //        return "";
        //    }
        //}
         
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public string[] url = new string[] { "https://news.google.com/news/rss/?ned=vi_vn&hl=vi", "https://news.google.com/news/rss/headlines/section/topic/WORLD.vi_vn/Th%E1%BA%BF%20gi%E1%BB%9Bi?ned=vi_vn&hl=vi", "https://news.google.com/news/rss/headlines/section/topic/BUSINESS.vi_vn/Kinh%20doanh?ned=vi_vn&hl=vi", "https://news.google.com/news/rss/headlines/section/topic/ENTERTAINMENT.vi_vn/Gi%E1%BA%A3i%20tr%C3%AD?ned=vi_vn&hl=vi","https://news.google.com/news/rss/headlines/section/topic/SPORTS.vi_vn/Th%E1%BB%83%20thao?ned=vi_vn&hl=vi"};
        public async Task<string> crawl()
        {
            if (Config.isCrawl) return "Crawling..";
            try{

                DateTime startTime = DateTime.Now;
                DateTime endTime = DateTime.Now;
                Config.isCrawl = true;
                for (int i = 0; i < url.Length; i++)
                {
                    XmlDocument RSSXml = new XmlDocument();

                    try
                    {

                        RSSXml.Load(url[i]);

                    }
                    catch (Exception ex)
                    {

                    }

                    XmlNodeList RSSNodeList = RSSXml.SelectNodes("rss/channel/item");
                    XmlNode RSSDesc = RSSXml.SelectSingleNode("rss/channel/title");

                    //StringBuilder sb = new StringBuilder();
                    try
                    {
                        foreach (XmlNode RSSNode in RSSNodeList)
                        {

                            try
                            {
                                XmlNode RSSSubNode;
                                RSSSubNode = RSSNode.SelectSingleNode("title");
                                string title = RSSSubNode != null ? RSSSubNode.InnerText : "";

                                RSSSubNode = RSSNode.SelectSingleNode("link");
                                string link = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                if (link.Contains("http://news.google.com"))
                                {
                                    link = link.Substring(link.IndexOf("&url=") + 5);
                                }
                                if (link.Contains("bbc.com")) continue;
                                RSSSubNode = RSSNode.SelectSingleNode("description");
                                string desc = RSSSubNode != null ? RSSSubNode.InnerText : "";

                                RSSSubNode = RSSNode.SelectSingleNode("pubDate");

                                string date = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                // Kiểm tra nếu ngày gửi quá lâu thì không lấy
                                string image = "";
                                try
                                {
                                    if (link.Contains("http://news.google.com"))
                                    {
                                        image = getImageSrc(desc);
                                    }
                                    else
                                    {
                                        RSSSubNode = RSSNode.SelectSingleNode("media:thumbnail");
                                        image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value : "";
                                    }
                                }
                                catch (Exception img1)
                                {

                                }


                                //try { 
                                //    RSSSubNode = RSSNode.SelectSingleNode("media:thumbnail");
                                //    image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value: "";
                                //}catch(Exception img1){

                                //}
                                if (image == "")
                                {
                                    try
                                    {
                                        RSSSubNode = RSSNode.SelectSingleNode("media:content");
                                        image = RSSSubNode != null ? RSSSubNode.Attributes["url"].Value : "";
                                    }
                                    catch (Exception img2)
                                    {

                                    }
                                }
                                //RSSSubNode = RSSNode.SelectSingleNode("maindomain");
                                //string maindomain = RSSSubNode != null ? RSSSubNode.InnerText : "";

                                //RSSSubNode = RSSNode.SelectSingleNode("catid");
                                //string catid = RSSSubNode != null ? RSSSubNode.InnerText : "";
                                //string all = title + " " + desc;
                                //for (int l = 0; l < stopword.Length; l++)
                                //{
                                //    all = all.Replace(stopword[l], "");
                                //}
                                //string state1 = "";
                                //string capital1 = "";
                                //string largestcity1 = "";
                                //for (int j = 0; j < States.Count; j++)
                                //{
                                //    state1 = "";
                                //    capital1 = "";
                                //    largestcity1 = "";
                                //    if (all.Contains(States[j].state))
                                //    {
                                //        state1 = States[j].state;
                                //    }
                                //    if (state1 != "" && all.Contains(States[j].capital))
                                //    {
                                //        capital1 = States[j].capital;
                                //    }
                                //    if (state1 != "" && all.Contains(States[j].largestcity))
                                //    {
                                //        largestcity1 = States[j].largestcity;
                                //    }
                                //    if (state1 != "") break;
                                //}
                                if (title != null && !title.Equals(""))// && state1 != ""
                                {
                                    link = link.Trim();
                                    title = title.Trim();
                                    int datetimeid = int.Parse(Config.convertToDateTimeId(date));

                                    if (datetimeid != 0)
                                    {
                                        DateTime? fdate = Config.toDateTime(date);
                                        //fdate = fdate.Value.AddHours(-12);
                                        var any = db.news.Any(o => o.date_id == datetimeid && o.name == title && o.link == link);
                                        if (!any)
                                        {
                                            Uri urldomain = new Uri(link);
                                            string pdf = Config.unicodeToNoMark(title) + ".pdf";
                                            savePdf(link, pdf, urldomain.Host);
                                            string full_content = "";
                                            try {
                                                Reader reader = new Reader();
                                                Article result = await reader.Read(new Uri(link));
                                                if (image == "")
                                                {
                                                    try { 
                                                        image = result.FrontImage.ToString();
                                                    }
                                                    catch (Exception img1)
                                                    {
                                                        try
                                                        {
                                                            image = result.Images.ElementAt(0).Uri.ToString();
                                                        }
                                                        catch (Exception img2)
                                                        {
                                                            image = "";
                                                        }
                                                    }
                                                }
                                                full_content = result.Content;
                                            }catch(Exception ex222){

                                            }
                                            news n = new news();
                                            n.date_id = datetimeid;
                                            n.date_time = fdate;
                                            n.des = desc;
                                            n.full_content = "";
                                            n.link = link;
                                            n.name = title;
                                            n.cat_id = i;
                                            n.full_content = full_content;
                                            n.image = image;
                                            n.time = fdate.Value.TimeOfDay;
                                            n.status = 0;
                                            n.loads = 1;
                                            string strDay = DateTime.Now.ToString("yyyyMMdd");
                                            string fullPath = "/Files/" + strDay + "/" + pdf;
                                            n.pdf = fullPath;
                                            db.news.Add(n);
                                            db.SaveChanges();

                                        }
                                    }

                                }
                                else continue;
                            }
                            catch (Exception exInFor1)
                            {
                                //int abc = 0;
                                //Array.Resize(ref arrItem, Length);
                            }
                        }//for node
                    
                    }
                    catch (Exception exTryFor2)
                    {
                        //int abc = 0;
                    }
                } //for           
                generateSiteMap();            
                endTime = DateTime.Now;            
                string howLong = Config.getDiffTimeMinuteFromTwoDate(startTime, endTime) + " Done All \r\n";
                howLong += "Start at: " + startTime.ToString() + ".\r\n";     
                howLong += "End at: " + DateTime.Now.ToString() + ".\r\n";            
                StreamWriter SW = new StreamWriter(HttpRuntime.AppDomainAppPath + "HowLong.txt");
                SW.WriteLine(howLong);
                SW.Close();
            }catch(Exception all){

            }
            Config.isCrawl = false;
            return "Done";
        }
        public void savePdf(string url, string name, string domain)
        {
            var originalDirectory = new DirectoryInfo(string.Format("{0}Files", Server.MapPath(@"\")));
            string strDay = DateTime.Now.ToString("yyyyMMdd");
            string pathString = System.IO.Path.Combine(originalDirectory.ToString(), strDay);
            string fullPath = pathString + "\\" + name;
            bool isExists = System.IO.Directory.Exists(pathString);
            if (!isExists)
                System.IO.Directory.CreateDirectory(pathString);
            if (System.IO.File.Exists(fullPath)) return;
            HtmlToPdf converter = new HtmlToPdf();
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(url);

            // get conversion result (contains document info from the web page)
            HtmlToPdfResult result = converter.ConversionResult;

            // set the document properties
            doc.DocumentInformation.Title = result.WebPageInformation.Title;
            doc.DocumentInformation.Subject = result.WebPageInformation.Description;
            doc.DocumentInformation.Keywords = result.WebPageInformation.Keywords;

            doc.DocumentInformation.Author = domain;
            doc.DocumentInformation.CreationDate = DateTime.Now;

            // save pdf document
            doc.Save(fullPath);

            // close pdf document
            doc.Close();
        }
        public string getImageSrc(string content)
        {
            string matchString = Regex.Match(content, "<img.*?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase).Groups[1].Value;

            return matchString;
        }
        public void generateSiteMap()
        {

            try
            {

                XmlWriterSettings settings = null;
                string xmlDoc = null;
                int date_id = Config.datetimeidByDay(-3);
                var p = (from q in db.news where q.full_content != null select q).ToList();
                settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                xmlDoc = HttpRuntime.AppDomainAppPath + "sitemap.xml";//HttpContext.Server.MapPath("../") + 
                float percent = 0.85f;

                string urllink = "";
                using (XmlTextWriter writer = new XmlTextWriter(xmlDoc, Encoding.UTF8))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://tintucmoinhat.vn/");
                    writer.WriteElementString("changefreq", "always");
                    writer.WriteElementString("priority", "1");
                    writer.WriteEndElement();

                    for (int i = 0; i < p.Count; i++)
                    {

                        try
                        {
                            //if (!System.IO.File.Exists(Server.MapPath(@"\") + p[i].pdf)) continue;
                            var link = "/" + TinTucMoiNhat.Config.getCatName(p[i].cat_id) + "/" + TinTucMoiNhat.Config.unicodeToNoMark(p[i].name) + "-" + p[i].id;
                            writer.WriteStartElement("url");
                            urllink = "http://tintucmoinhat.vn" + link;
                            writer.WriteElementString("loc", urllink);
                            //writer.WriteElementString("lastmod", DR["datetime"].ToString());
                            try
                            {
                                if (i < 500)
                                {
                                    writer.WriteElementString("changefreq", "hourly");
                                    percent = 0.85f;
                                }
                                else
                                {
                                    writer.WriteElementString("changefreq", "monthly");
                                    percent = 0.70f;
                                }
                            }
                            catch (Exception ex1)
                            {
                            }

                            writer.WriteElementString("priority", percent.ToString("0.00"));
                            writer.WriteEndElement();
                        }
                        catch (Exception ex2)
                        {
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

            }
            catch (Exception extry)
            {
                //StreamWriter sw = new StreamWriter();
            }

        }
    }
}