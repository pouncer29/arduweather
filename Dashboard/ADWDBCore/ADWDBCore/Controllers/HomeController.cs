using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class DataSelect
    {
        public string Timeline { get; set; }
        public DataPoint TemperatureBox{ get; set; }
        public DataPoint HumidityBox { get; set; }
        public DataPoint BrightnessBox{ get; set; }
        public DataPoint WindSpeedBox{ get; set; }

        public DataPoint points
        {
            get
            {
                return TemperatureBox | HumidityBox | BrightnessBox | WindSpeedBox;
            }
        }

        public override string ToString()
        {
            return
                $"Timeline: {Timeline} \n" +
                $"Temperature {TemperatureBox}, Humidity {HumidityBox}, Brightness {BrightnessBox}, WindSpeed {WindSpeedBox}";
        }
    }
    
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
            ViewBag.Humidity = dbMan.LatestHumidity;
            ViewBag.Brightness = dbMan.LatestBrightness;
            ViewBag.WindSpeed = dbMan.LatestWindSpeed;
            ViewBag.WindDir = dbMan.LatestWindDir;
            ViewBag.Timestamp = dbMan.LatestTimestampFriendly;
            return PartialView("Index");
        }

        [HttpPost]
        //[ActionName("DrawChartX")]
        [Route("Index/Charts/DrawChartX")]
        public ActionResult DrawChartX(DataSelect data)
        {
            Console.WriteLine(data.ToString());
            List<Object> dataList = null;
            if (data.Timeline.Equals("Week"))
                dataList = dbMan.GetWeeklyChart(data.points);
            else if (data.Timeline.Equals("Month"))
                dataList = dbMan.GetMontlyChart(data.points);
            else if (data.Timeline.Equals("Year"))
                dataList = dbMan.GetYearlyChart(data.points);
            else
                dataList = dbMan.GetDailyChart(data.points);

             var convertedJson = JsonConvert.SerializeObject(dataList, new JsonSerializerSettings()
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
            ViewBag.Humidity = dbMan.LatestHumidity;
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
        
        
        #region Charts

        [Route("Index/Charts/Test")]
        public ActionResult OnGetChartData()
        {
            var dummyList = Weather_Chart.GetDummyChart();
            var legitList = (dbMan as DBManager_Mongo).GetPastNDaysChart(60);
            var convertedJson = JsonConvert.SerializeObject(legitList, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return Content(convertedJson);
        }
       
        #region all
        [Route("Index/Charts/All/Year")]
        public ActionResult GetChartData_All_Year()
        {
            var legitList = dbMan.GetYearlyChart();
            var convertedJson = JsonConvert.SerializeObject(legitList, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return Content(convertedJson);
        }
        
         [Route("Index/Charts/All/Month")]
          public ActionResult GetChartData_All_Month()
          {
             var legitList = dbMan.GetMontlyChart();
             var convertedJson = JsonConvert.SerializeObject(legitList, new JsonSerializerSettings()
             {
                 NullValueHandling = NullValueHandling.Ignore
             });
 
             return Content(convertedJson);
         }
          
          [Route("Index/Charts/All/Week")]
          public ActionResult GetChartData_All_Week()
          {
              //points = DataPoint.temperature | DataPoint.humidity;
              var legitList = dbMan.GetWeeklyChart();
              var convertedJson = JsonConvert.SerializeObject(legitList, new JsonSerializerSettings()
              {
                  NullValueHandling = NullValueHandling.Ignore
              });

              return Content(convertedJson);
          } 


          [Route("Index/Charts/All/Day")]
          public ActionResult GetChartData_All_Day()
          {
              var legitList = dbMan.GetDailyChart();
              var convertedJson = JsonConvert.SerializeObject(legitList, new JsonSerializerSettings()
              {
                  NullValueHandling = NullValueHandling.Ignore
              });

              return Content(convertedJson);
          }

          #endregion
          
          
          #region 

          

          #endregion



          #endregion Charts
        
        
        
        
    }
}