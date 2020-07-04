﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ADWDBCore.Models;
using DBConstants;
using DBMan;
using DBManager.JsonHelpers;
using MongoDB.Bson;
using Newtonsoft.Json;

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

        [Route("Index/Charts/Test")]
        public ActionResult OnGetChartData()
        {
            var dummyList = Weather_Chart.GetDummyChart();
            var convertedJson = JsonConvert.SerializeObject(dummyList, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return Content(convertedJson);
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
            //ViewBag.WeekDeets = (dbMan as DBManager_Mongo).weatherChartData(new List<BsonDocument>(),DataPoint.all);
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