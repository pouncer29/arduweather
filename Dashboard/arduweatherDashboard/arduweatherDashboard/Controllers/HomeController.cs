using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using arduweatherDashboard.Models;

namespace arduweatherDashboard.Controllers
{
    public class HomeController : Controller
    {

        private DBManager manager;

        public HomeController() : base()
        {
            manager = new DBManager();
        }
        public ActionResult Index()
        {
            ViewBag.Temp = manager.LatestTemp;
            ViewBag.Humidity= manager.LatestHumidity;
            ViewBag.WindSpeed= manager.LatestWindSpeed;
            ViewBag.Brightness = manager.LatestBrightness;
            ViewBag.Timestamp = manager.LatestTimestamp;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "arduweather Dashboard";
            ViewBag.Boogers = "I am a booger";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}