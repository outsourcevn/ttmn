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
using Facebook;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Boilerpipe;
using Boilerpipe.Net.Extractors;
namespace TinTucMoiNhat.Controllers
{
    public class HomeController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();
        public static string _accessToken_ = "";
        public static FacebookClient fbApi = null;
        public static FacebookClient fbApi2 = null;

        public ActionResult Index(string catname, int? cat_id, int? pg)
        {
            if (cat_id == null) cat_id = 0;
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            int date_id = Config.datetimeidByDay(-10);
            var p = (from q in db.news where q.cat_id == cat_id && q.date_id >= date_id select q).OrderByDescending(o => o.date_time).ToList();
            var p2 = (from q2 in db.news where q2.date_id >= date_id select q2).OrderByDescending(o => o.date_time).Take(15).ToList();
            ViewBag.news = p2;
            var p3 = (from q3 in db.news where q3.cat_id == 0 && q3.date_id >= date_id select q3).OrderByDescending(o => o.loads).ThenByDescending(o => o.date_time).Take(10).ToList();
            ViewBag.ppl = p3;
            ViewBag.catname = Config.getCatName(cat_id);
            ViewBag.catfullname = Config.getFullCatName(cat_id);
            ViewBag.cat_id = cat_id;
            ViewBag.pg = pg;
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
        public ActionResult Fanpage(string catname, string page_id, int? pg)
        {
            if (page_id == null || page_id=="0") page_id = "";
            if (catname == null || catname == "") catname = "all";
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            int date_id = Config.datetimeidByDay(-10);
            var p = (from q in db.posts where q.page_id.Contains(page_id) && q.date_id >= date_id select q).OrderByDescending(o => o.date_post).ToList();
            var p2 = (from q2 in db.pages select q2).OrderByDescending(o => o.name).ToList();
            ViewBag.news = p2;
            ViewBag.catfullname = Config.getFanpageName(page_id);
            ViewBag.catname = Config.unicodeToNoMark(ViewBag.catfullname);
            ViewBag.page_id = page_id!=""?page_id:"0";
            ViewBag.pg = pg;
            return View(p.ToPagedList(pageNumber, pageSize));
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
                                if (link.Contains("bbc.com") || link.Contains("rfa.org") || link.Contains("vtimes.com.au")) continue;//link.Contains("kenh14.vn") || link.Contains("tuoitre.vn")
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
                                            //Uri urldomain = new Uri(link);
                                            //string pdf = Config.unicodeToNoMark(title) + ".pdf";
                                            //savePdf(link, pdf, urldomain.Host);
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
                                            if (full_content == "") { full_content = getAllContentByAi3(link); }
                                            try {
                                                if (image == "" || link.Contains("soha.vn") || link.Contains("zing.vn"))
                                                {
                                                    image = getImageFromAllContent(link);
                                                }
                                            }catch(Exception images){

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
                                            //string strDay = DateTime.Now.ToString("yyyyMMdd");
                                            //string fullPath = "/Files/" + strDay + "/" + pdf;
                                            n.pdf = null;
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
                crawlFacebook();
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
        public string getAllContentByAi3(string link)
        {
            try { 
                String url = link;
                string allImage = "";
                String page = String.Empty;
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                page = streamReader.ReadToEnd();
                string pattern = @"<(img)\b[^>]*>";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(page);

                for (int i = 0, l = matches.Count; i < l; i++)
                {

                    allImage += matches[i].Value;
                }
                string text = CommonExtractors.ArticleExtractor.GetText(page);
                return text + allImage;
            }
            catch
            {
                return "";
            }
        }
        public string crawlPdf()
        {
            if (Config.isCrawlPdf) return "Crawling";
            try
            {
              Config.isCrawlPdf = true;
              int datetimeid = Config.datetimeidByDay(-1);
              var p = (from q in db.news where q.date_id>=datetimeid && q.pdf == null select q).ToList();
              for (int j = 0; j < p.Count; j++)
              {
                  string link = p[j].link;
                  Uri urldomain = new Uri(link);
                  string pdf = Config.unicodeToNoMark(p[j].name) + ".pdf";
                  savePdf(link, pdf, urldomain.Host);
                  string strDay = DateTime.Now.ToString("yyyyMMdd");
                  string fullPath = "/Files/" + strDay + "/" + pdf;
                  db.Database.ExecuteSqlCommand("update news set pdf=N'" + fullPath + "' where id=" + p[j].id);
              }
                
            } catch(Exception pdf){

            }
            Config.isCrawlPdf = false;
            return "Done";
        }
        public string getImageFromAllContent(string link)
        {
            try
            {
                String page = String.Empty;
                WebRequest request = WebRequest.Create(link);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                page = streamReader.ReadToEnd();
                string pattern = @"<(img)\b[^>]*>";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                MatchCollection matches = rgx.Matches(page);
                return getImageSrc(matches[0].Value);

            }catch(Exception all){
                return "";
            }
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
        public class channel
        {
            public string name { get; set; }
            public string domain { get; set; }
            public string id { get; set; }
            public int cat_id { get; set; }
        }
        public class Posts
        {
            public string message { get; set; }
            public string created_time { get; set; }
            public string id { get; set; }
            public string post_link { get; set; }
            public string image { get; set; }
            public string url { get; set; }
            //public string PostPicture { get; set; }

            //public string UserId { get; set; }
            //public string UserName { get; set; }

        }
        public class SinglePosts
        {
            public string media { get; set; }
            public string url { get; set; }
            public string description { get; set; }


        }
        public String GetAccessToken()
        {
            //create the constructor with post type and few data
            //HttpWebRequest = new HttpWebRequest("https://graph.facebook.com/oauth/access_token", "GET", "client_id=" + this.ApplicationID + "&client_secret=" + this.ApplicationSecret + "&code=" + code + "&redirect_uri=http:%2F%2Flocalhost:5176%2F");
            if (_accessToken_ != "") return _accessToken_;
            WebClient wc = new WebClient();
            string test = wc.DownloadString("https://graph.facebook.com/oauth/access_token?client_id=1930351307185969&client_secret=8041e365a07363d325c8aa2795d7bacb&grant_type=client_credentials");
            //string accessToken = JsonConvert.DeserializeObject(test).//test.Split('&')[0];
            dynamic array = JsonConvert.DeserializeObject(test);
            //accessToken = accessToken.Split('=')[1];
            //_accessToken_ = accessToken;
            return array.access_token;
        }
        public string getPostById(string id)
        {
            string accessToken = GetAccessToken();//		accessToken	"904499149629830|35rV5y3FzqosopNwjHxomziGMTk"	string
            if (fbApi == null) fbApi = new FacebookClient(accessToken);
           
            dynamic result = fbApi.Get("/" + id + "/attachments");
           
            if (result == null)
            {
                result = fbApi.Get("/" + id + "/subattachments");
            }
            return JsonConvert.SerializeObject(result);
        }
        private static IEnumerable<JToken> AllChildren(JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in AllChildren(c))
                {
                    yield return cc;
                }
            }
        }
        public static string getRealLink(string url)
        {
            if (url == null || url == "") return "";
            try
            {
                if (url.Contains("l.php?u="))
                {
                    Regex titRegex = new Regex(@"(?<=u=)(.*)(?=&h=.)", RegexOptions.IgnoreCase);
                    Match titm = titRegex.Match(url);
                    if (titm.Success)
                    {
                        url = HttpUtility.UrlDecode(titm.Groups[0].Value);
                    }
                    else return "";
                }
                return url;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GetPictureUrl(string faceBookId)
        {
            WebResponse response = null;
            string pictureUrl = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(string.Format("https://graph.facebook.com/{0}/picture", faceBookId));
                response = request.GetResponse();
                pictureUrl = response.ResponseUri.ToString();
            }
            catch (Exception ex)
            {
                //? handle
            }
            finally
            {
                if (response != null) response.Close();
            }
            return pictureUrl;
        }
        public string getAvatar(string id)
        {
            string accessToken = GetAccessToken();
            if (fbApi == null) fbApi = new FacebookClient(accessToken);

            dynamic result = fbApi.Get("/" + id + "/picture");

            return JsonConvert.SerializeObject(result);
        }
        public void crawlFacebook()
        {
            try { 
                //string query = "delete from posts where date_id<" + Config.datetimeidaddday(-60);
                //db.Database.ExecuteSqlCommand(query);
            
                DateTime fromTime = DateTime.Now;
                List<channel> cnns = new List<channel>();
                channel cn = new channel();
                var pl = (from q in db.pages where q.status == 0 select q).OrderBy(o => o.cat_id).ToList();
                for (int l = 0; l < pl.Count; l++)
                {
                    cn = new channel();
                    cn.domain = pl[l].domain;
                    cn.name = pl[l].name;
                    cn.id = pl[l].page_id;
                    cn.cat_id = (int)pl[l].cat_id;
                    cnns.Add(cn);
                }        


                //Crawl
                string accessToken = GetAccessToken();//		accessToken	"904499149629830|35rV5y3FzqosopNwjHxomziGMTk"	string
                if (fbApi2 == null) fbApi2 = new FacebookClient(accessToken);
                post newpost = new post();
                StringBuilder log = new StringBuilder();
                for (int item = 0; item < cnns.Count; item++)
                {
                
                    try
                    {
                        dynamic result = fbApi2.Get("/" + cnns[item].id + "?fields=posts");
                        dynamic p = result.posts.data;
                        //List<Posts> postsList = new List<Posts>();
                        for (int i = 0; i < p.Count; i++)
                        {
                            if (p[i].message == null || p[i].message == "") continue;
                            //Posts posts = new Posts();
                            string posts_id = p[i].id;
                            string image = "";
                            string link = "";
                            string page_id = cnns[item].id;
                            posts_id = posts_id.Split('_')[1];
                            int date_id = int.Parse(Config.convertToDateTimeId(p[i].created_time));
                            long date_long = Config.convertToLongType(p[i].created_time);
                            string json = getPostById(p[i].id);
                            var resultObjects = AllChildren(JObject.Parse(json))
                            .First(c => c.Type == JTokenType.Array && c.Path.Contains("data"))
                            .Children<JObject>();
                            bool found = false;
                            foreach (JObject item2 in resultObjects)
                            {
                                //foreach (JProperty property in item.Properties())
                                //{
                                //    string image = property["image"].ToString();
                                //}
                                try
                                {
                                    if (!json.Contains("subattachments"))
                                    {
                                        image = item2["media"]["image"]["src"].ToString();
                                        link = getRealLink(item2["url"].ToString());
                                    }
                                    else
                                    {
                                        image = (string)item2["subattachments"]["data"][0]["media"]["image"]["src"];
                                        link = getRealLink((string)item2["target"]["url"]);
                                    }
                                    if (link != "") found = true;
                                }
                                catch (Exception ex3)
                                {
                                    found = false;
                                }
                                break;
                            }
                            string avatar = "";
                            avatar = GetPictureUrl(page_id);//getAvatar(page_id);
                            //if (!found) continue;
                            //posts.message = p[i].message;
                            //posts.created_time = p[i].created_time;
                            //posts.post_link = Config.embededPost("https://www.facebook.com/haivl.com/posts/", posts.id.Split('_')[1], posts.message, "https://www.facebook.com/haivl.com", "Góc thư giãn", posts.created_time);// +"/posts/140549259338782" + posts.id.Split('_')[1];//https://www.facebook.com/SEAHub.org/
                            //postsList.Add(posts);
                            string pdomain = cnns[item].domain;
                            bool check = db.posts.Any(o => o.date_id == date_id && o.post_id.Contains(posts_id) && o.domain.Contains(pdomain));
                            if (!check)
                            {
                                newpost = new post();
                                newpost.post_id = posts_id;
                                newpost.cat_id = cnns[item].cat_id;
                                newpost.url = cnns[item].domain + "/posts/";
                                newpost.message = p[i].message;
                                newpost.domain = cnns[item].domain;
                                newpost.domain_name = cnns[item].name;
                                newpost.create_time = p[i].created_time;
                                newpost.date_post = Config.convertDateTime(newpost.create_time);
                                newpost.date_id = date_id;
                                newpost.image = image;
                                newpost.link = link;
                                newpost.page_id = page_id;
                                newpost.date_long = date_long.ToString();
                                newpost.avatar = avatar;
                                db.posts.Add(newpost);
                                db.SaveChanges();
                            }
                            else
                            {
                                string update = "update posts set avatar=N'" + avatar + "',date_long=" + date_long.ToString() + ",page_id=N'" + page_id + "',link=N'" + link + "',image=N'" + image + "' where date_id = " + date_id + "  and post_id='" + posts_id + "' and domain='" + pdomain + "'";
                                db.Database.ExecuteSqlCommand(update);
                            }
                        }
                    }
                    catch (Exception ex22222)
                    {
                       
                    }
                }
            }
            catch
            {

            }
        }
       
        public void generateSiteMap()
        {

            try
            {

                XmlWriterSettings settings = null;
                string xmlDoc = null;
                int date_id = Config.datetimeidByDay(-30);
                var p = (from q in db.news where q.date_id>=date_id && q.full_content != null select q).OrderByDescending(o=>o.date_time).ToList();
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
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "http://tintucmoinhat.vn/fanpage/all-0/1");
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