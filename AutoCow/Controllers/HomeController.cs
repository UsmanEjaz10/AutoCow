using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;

namespace AutoCow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductionRepository _productionRepository;
        private readonly Daily_planRepository _daily_planRepository;
        private readonly MachineLearning _machineLearningRepository;

        public HomeController()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _productionRepository = new ProductionRepository(connectionString);
            _daily_planRepository = new Daily_planRepository(connectionString);
            _machineLearningRepository = new MachineLearning(connectionString);
        }

        [Route("")]
        public ActionResult Index()
        {

            //------------------------------------------------------------------------------------//

            List<ProductionData> productionDataList = _productionRepository.GetProductionData();

            // Convert the dataset to arrays for use in the view //
            var dates = productionDataList.Select(d => d.Date.ToShortDateString()).ToArray();
            var milkProductions = productionDataList.Select(d => d.MilkProduction).ToArray();

            //Setting the arrays in view Bag //
            ViewBag.Dates = JsonConvert.SerializeObject(dates);
            ViewBag.MilkProductions = JsonConvert.SerializeObject(milkProductions);

            //---------------------------------------------------------------------------------------//

            List<Daily_plan> daily_planList = _daily_planRepository.GetDailyData();

            var bar_dates = daily_planList.Select(d => d.date.ToShortDateString()).ToArray();
            var bar_milk = daily_planList.Select(d => d.total).ToArray();
            var bar_types = daily_planList.Select(d => d.type).ToArray();

            ViewBag.bar_dates = JsonConvert.SerializeObject(bar_dates);
            ViewBag.bar_milk = JsonConvert.SerializeObject(bar_milk);
            ViewBag.bar_types = JsonConvert.SerializeObject(bar_types);

            //------------------------------------------------------------------------------------------//

            List<Daily_plan> condition_countList = _daily_planRepository.GetConditionCount();

            var alert_conditions = condition_countList.Select(d => d.condition).ToArray();
            var alert_counts = condition_countList.Select(d => d.count_condition).ToArray();

            ViewBag.alert_conditions = JsonConvert.SerializeObject(alert_conditions);
            ViewBag.alert_counts = JsonConvert.SerializeObject(alert_counts);

            ViewBag.averageProduction = _productionRepository.averageProduction();

            //-----------------------------------------------------------------------------------------//

            List<Daily_plan> leaderboardlist = _daily_planRepository.getLeaderboard();
            //var leaderboard_dates = leaderboardlist.Select(d => d.id).ToArray();
            //var leaderboard_milk = leaderboardlist.Select(d => d.milk).ToArray();
            //var leaderboard_type = leaderboardlist.Select(d => d.type).ToArray();

            ViewBag.leaderboard = leaderboardlist;


            return View();
        }

        [Route("privacy")]
        public ActionResult Privacy() {

            var context = new MLContext();
            var productionData = _productionRepository.GetAllTimeProductionData();
            foreach(var i in productionData)
            {
                Console.WriteLine(i.MilkProduction);
            }
            var dataView = context.Data.LoadFromEnumerable(productionData);

            var pipline = context.Forecasting.ForecastBySsa(
                "Forecast",
                nameof(ProductionData.MilkProduction),
                windowSize: 2,
                seriesLength: 3,
                trainSize: 5,
                horizon: 10
                ) ;
            var model = pipline.Fit(dataView);
            var forecastingEngine = model.CreateTimeSeriesEngine<ProductionData, ProductionPredictor>(context);
            var forecasts = forecastingEngine.Predict();

            foreach(var i in forecasts.Forecast)
            {
                Console.WriteLine(i);
            }


            return View();
        }


        [Route("dashboard")]
        public ActionResult Dashboard() {

			List<Daily_plan> leaderboardlist = _daily_planRepository.getLeaderboard();
			var leaderboard_dates = leaderboardlist.Select(d => d.id).ToArray();
			var leaderboard_milk = leaderboardlist.Select(d => d.milk).ToArray();
			var leaderboard_type = leaderboardlist.Select(d => d.type).ToArray();

			ViewBag.leaderboard = leaderboardlist;

			return View();
        }

        [Route("d")]
        public ActionResult Index1() { return View(); }
    }




}