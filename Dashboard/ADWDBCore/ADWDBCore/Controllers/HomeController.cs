using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ADWDBCore.Models;
using DBMan;

namespace ADWDBCore.Controllers
{
    
     public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IDBManager dbMan;

        public HomeController(ILogger<HomeController> logger)
        {
            dbMan = new DBManager_Mongo();
            dbMan.NewEntry += this.callViewDeets;
            _logger = logger;
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
            ViewBag.Brightness = dbMan.LatestBrightness;
            ViewBag.WindSpeed = dbMan.LatestWindSpeed;
            ViewBag.WindDir = dbMan.LatestWindDir;
            ViewBag.Timestamp = dbMan.LatestTimestampFriendly;
            return PartialView("Index");
        }

        [HttpGet]
        public ActionResult Index()
        {
            //var dbMan = new DBManager_Mongo();
            ViewBag.Timestamp = dbMan.LatestTimestamp;
            ViewBag.Temperature = dbMan.LatestTemperature;
            ViewBag.Humidity = dbMan.LatestTemperature;
            ViewBag.Brightness = dbMan.LatestBrightness;
            ViewBag.WindSpeed = dbMan.LatestWindSpeed;
            ViewBag.WindDir = dbMan.LatestWindDir;
            ViewBag.Timestamp = dbMan.LatestTimestampFriendly;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}