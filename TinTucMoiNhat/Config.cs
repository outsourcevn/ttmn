using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using TinTucMoiNhat.Models;
using System.Linq;
namespace TinTucMoiNhat
{
    public class Config
    {
        public static bool isCrawl = false;
        public static bool isCrawlPdf = false;
        public static bool isCrawlFacebook = false;
        public static bool isCrawlYoutube = false;
        public static bool isCrawlFacebookNews = false;
        public static tintucmoinhatEntities db = new tintucmoinhatEntities(); 
        public static string convertToDateTimeId(string d)
        {
            DateTime d1;
            try
            {
                d1 = DateTime.Parse(d);//ToUniversalTime();
                return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
            }
            catch (Exception ex)
            {
                //d1 = DateTime.Now;
                //return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return "0";
            }
            //d1 = DateTime.Now;
            //return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
        }
        public static int todateTimeId(DateTime d)
        {
            DateTime d1;
            try
            {
                d1 = d;//ToUniversalTime();
                string val=d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(val);
            }
            catch (Exception ex)
            {
                d1 = DateTime.Now;//ToUniversalTime();
                string val = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(val);
            }
            //d1 = DateTime.Now;
            //return d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
        }
        public static string smoothTitle(string message)
        {
            try
            {
                string[] temp = message.Split(' ');
                int i=0;
                string val="";
                while (val.Length < 120 && i<temp.Length)
                {
                    val += temp[i]+" ";
                    i = i + 1;
                }
                return val.Trim();
            }
            catch (Exception ex)
            {
                return "chi-tiet";
            }
        }
        public static long convertToLongType(string d)
        {
            DateTime d1;
            try
            {
                d1 = DateTime.Parse(d);//ToUniversalTime();
                return d1.Ticks;
            }
            catch (Exception ex)
            {
                d1 = DateTime.Now;
                return d1.Ticks;
            }
        }
        public static DateTime convertDateTime(string val)
        {
            try
            {
                return DateTime.Parse(val);
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        public static string toUTCTime(string d)
        {
            string d1;
            try
            {
                d1 = DateTime.Parse(d).ToUniversalTime().ToString();
            }
            catch (Exception ex)
            {
                return DateTime.Now.ToUniversalTime().ToString();
            }
            return d1;
        }
        public static DateTime? toDateTime(string d)
        {
            //if (d.Contains("GMT")) d = d.Replace("GMT", "");
            DateTime? d1;
            try
            {
                d1 = DateTime.Parse(d);
            }
            catch (Exception ex)
            {
                return null;
            }
            return d1;
        }
        public static int datetimeidByDay(int days)
        {
            DateTime d1;
            try
            {
               
                d1 = DateTime.Now.AddDays(days);//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);

            }
            catch (Exception ex)
            {
                d1 = DateTime.Now.AddDays(days);//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
            //return DateTime.Now.Year * 365 + DateTime.Now.Month * 30 + DateTime.Now.Day;
        }
        public static int datetimeid()
        {
            DateTime d1;
            try
            {

                d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);

            }
            catch (Exception ex)
            {
                d1 = DateTime.Now;//.ToUniversalTime();
                string rs = d1.Year.ToString() + d1.Month.ToString("00") + d1.Day.ToString("00");
                return int.Parse(rs);
            }
        }
        public static string unicodeToNoMark(string input)
        {
            input = input.ToLowerInvariant().Trim();
            if (input == null) return "";
            string noMark = "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,e,u,u,u,u,u,u,u,u,u,u,u,u,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,o,i,i,i,i,i,i,y,y,y,y,y,y,d,A,A,E,U,O,O,D";
            string unicode = "a,á,à,ả,ã,ạ,â,ấ,ầ,ẩ,ẫ,ậ,ă,ắ,ằ,ẳ,ẵ,ặ,e,é,è,ẻ,ẽ,ẹ,ê,ế,ề,ể,ễ,ệ,u,ú,ù,ủ,ũ,ụ,ư,ứ,ừ,ử,ữ,ự,o,ó,ò,ỏ,õ,ọ,ơ,ớ,ờ,ở,ỡ,ợ,ô,ố,ồ,ổ,ỗ,ộ,i,í,ì,ỉ,ĩ,ị,y,ý,ỳ,ỷ,ỹ,ỵ,đ,Â,Ă,Ê,Ư,Ơ,Ô,Đ";
            string[] a_n = noMark.Split(',');
            string[] a_u = unicode.Split(',');
            for (int i = 0; i < a_n.Length; i++)
            {
                input = input.Replace(a_u[i], a_n[i]);
            }
            input = input.Replace("  ", " ");
            input = Regex.Replace(input, "[^a-zA-Z0-9% ._]", string.Empty);
            input = removeSpecialChar(input);
            input = input.Replace(" ", "-");
            input = input.Replace("--", "-");
            return input;
        }
        public static string removeSpecialChar(string input)
        {
            input = input.Replace("-", "").Replace(":", "").Replace(",", "").Replace("_", "").Replace("'", "").Replace("\"", "").Replace(";", "").Replace("”", "").Replace(".", "").Replace("%", "");
            return input;
        }
        public static string getCatName(int? cat_id)
        {
            string val = "tin-nong";
            switch(cat_id){
                case 0:
                    val = "tin-nong";
                    break;
                case 1:
                    val = "the-gioi";
                    break;
                case 2:
                    val = "kinh-doanh";
                    break;
                case 3:
                    val = "giai-tri";
                    break;
                case 4:
                    val = "the-thao";
                    break;
                case 5:
                    val = "ha-noi";
                    break;
                case 6:
                    val = "tp-sai-gon-ho-chi-minh";
                    break;
                case 7:
                    val = "da-nang";
                    break;
                case 8:
                    val = "hai-phong";
                    break;
                case 9:
                    val = "can-tho";
                    break;
                case 10:
                    val = "quang-ninh";
                    break;
            }
            return val;
        }
        public static string getFullCatName(int? cat_id)
        {
            string val = "Tin nóng";
            switch (cat_id)
            {
                case 0:
                    val = "Tin nóng";
                    break;
                case 1:
                    val = "Thế giới";
                    break;
                case 2:
                    val = "Kinh doanh";
                    break;
                case 3:
                    val = "Giải trí";
                    break;
                case 4:
                    val = "Thể thao";
                    break;
                case 5:
                    val = "Báo Hà Nội";
                    break;
                case 6:
                    val = "Báo Tp Hồ Chí Minh";
                    break;
                case 7:
                    val = "Báo Đà Nẵng";
                    break;
                case 8:
                    val = "Báo Hải Phòng";
                    break;
                case 9:
                    val = "Báo Cần Thơ";
                    break;
                case 10:
                    val = "Báo Quảng Ninh";
                    break;
            }
            return val;
        }
        public static string getFullCatNameFacebook(int? cat_id)
        {
            string val = "Ngôi Sao, Giải Trí";
            switch (cat_id)
            {
                case 1:
                    val = "Ngôi Sao, Giải Trí";
                    break;
                case 6:
                    val = "Sản Phẩm, Dịch Vụ";
                    break;
                default: val = "Ngôi Sao, Giải Trí";
                    break;
            }
            return val;
        }
        public static string getFanpageName(string page_id){
            try{
                if (page_id == "") return "all";
                string p = db.pages.Where(o => o.page_id == page_id).FirstOrDefault().name;
                return p;
            }catch
            {
                return "";
            }
        }
        public static string getChannelName(int page_id)
        {
            try
            {
                if (page_id == 0) return "all";
                string p = db.channels.Where(o => o.id == page_id).FirstOrDefault().name;
                return p;
            }
            catch
            {
                return "";
            }
        }
        public static string getHost(string link)
        {
            try
            {
                Uri host = new Uri(link);
                return host.Host;
            }
            catch
            {
                return "Google Tin Tức";
            }
        }
        public static string getDiffTimeMinuteFromTwoDate(DateTime date1, DateTime date2)
        {
            try
            {
                DateTime d1 = date1;
                DateTime d2 = date2;
                TimeSpan TS = new System.TimeSpan(d2.Ticks - d1.Ticks);
                int totalHours = (int)Math.Abs(TS.TotalSeconds);
                if (totalHours < 0)
                {
                    return "1 phút trước";
                    //d2 = d2.ToLocalTime();
                    //return d2.Day + "/" + d2.Month + "/" + d2.Year;
                }
                else
                {
                    if (totalHours >= 3600)
                    {
                        int days = totalHours / 3600;
                        return days + " giờ trước";
                    }
                    else return totalHours.ToString() + " giây trước";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string embededPost(string url, string id, string message, string domain, string domainname, string createtime)
        {
            string temp = "";
            temp = "<div class=\"fb-post\" data-href=\"" + url + id + "\" data-width=\"500\"><div class=\"fb-xfbml-parse-ignore\"><blockquote cite=\"" + url + id + "\"><p>" + message + "</p>Posted by <a href=\"" + domain + "\">" + domainname + "</a> on&nbsp;<a href==\"" + url + id + "\">" + createtime + "</a></blockquote></div></div>";
            return temp;
        }
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = value;
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[v].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static void removeCookie(string field)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = "1";
            MyCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);           
        }
    }
    
}