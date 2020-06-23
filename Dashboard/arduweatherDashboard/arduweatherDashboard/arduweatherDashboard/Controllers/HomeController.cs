using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBMan;

namespace arduweatherDashboard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var dbMan = new DBManager_Mongo();
            ViewBag.Timestamp = dbMan.LatestTimestamp;
            ViewBag.Temperature = dbMan.LatestTemperature;
            ViewBag.Humidity = dbMan.LatestTemperature;
            ViewBag.Summary = dbMan.SummaryString();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}