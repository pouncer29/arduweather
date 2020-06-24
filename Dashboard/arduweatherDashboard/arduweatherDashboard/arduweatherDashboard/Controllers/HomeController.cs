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
        private IDBManager dbMan;
        public HomeController() : base()
        {
            dbMan = new DBManager_Mongo();
            dbMan.NewEntry += this.callViewDeets;
        }

        private void callViewDeets(object sender, EventArgs e)
        {
            this.UpdateViewDeets();
        }

        [HttpPost]
        public ActionResult UpdateViewDeets()
        {
             ViewBag.Timestamp = dbMan.LatestTimestamp;
             ViewBag.Temperature = dbMan.LatestTemperature;
             ViewBag.Humidity = dbMan.LatestTemperature;
             ViewBag.Summary = dbMan.SummaryString();
             return PartialView("Index");

        }

        [HttpGet]
        public ActionResult Index()
        {
            //var dbMan = new DBManager_Mongo();
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