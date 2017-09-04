using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TinTucMoiNhat.Models;
namespace TinTucMoiNhat.Controllers
{
    public class profilesController : Controller
    {
        private tintucmoinhatEntities db = new tintucmoinhatEntities();
        // GET: profiles
        public ActionResult Index()
        {
            return View();
        }
        public bool checkDuplicate(int type,string val){
            return db.profiles.Any(o=>o.email==val || o.phone==val);
        }
    }
}