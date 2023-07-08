// DESKTOP-IPO0A9V\SQLEXPRESS //

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
        private readonly Monthly_ProductionRepository _monthly_productionRepository;

		private readonly Cow_ProfileRepository _cow_ProfileRepository;
		public HomeController()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _productionRepository = new ProductionRepository(connectionString);
            _daily_planRepository = new Daily_planRepository(connectionString);
            _machineLearningRepository = new MachineLearning(connectionString);
            _cow_ProfileRepository = new Cow_ProfileRepository(connectionString);
			_monthly_productionRepository = new Monthly_ProductionRepository(connectionString);
        }

        [Route("")]
        public IActionResult Index()
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
            ViewBag.leaderboard = leaderboardlist;

            //-----------------------------------------------------------------------------------------//

            int NB = _cow_ProfileRepository.getCount("Normal Breed");
			int CB = _cow_ProfileRepository.getCount("Cross Breed");

            ViewBag.NB = NB;
            ViewBag.CB = CB;
			//-----------------------------------------------------------------------------------------//


			return View();
        }

        [Route("privacy")]
        public IActionResult Privacy() {

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
        public IActionResult Dashboard() {

			List<Daily_plan> leaderboardlist = _daily_planRepository.getLeaderboard();
			var leaderboard_dates = leaderboardlist.Select(d => d.id).ToArray();
			var leaderboard_milk = leaderboardlist.Select(d => d.milk).ToArray();
			var leaderboard_type = leaderboardlist.Select(d => d.type).ToArray();

			ViewBag.leaderboard = leaderboardlist;

			return View();
        }

        [Route("d")]
        public ActionResult Index1() { return View(); }



        [Route("predictions")]
        public IActionResult Predictions()
        {
            DateTime d = DateTime.Now;
            List<String> months = new List<String>();
            
            for(int i = 0; i< 12; i++)
            {
                DateTime e = d.AddMonths(i);

                months.Add(e.ToString("MMM"));

                Console.WriteLine(months[i]);
            }

            // Load model and predict the next set values.
            // The number of values predicted is equal to the horizon specified while training.
            var result = MLModel1.Predict(null, 12);

            
            var upperbound = JsonConvert.SerializeObject( result.Milk_Production_UB);
            var lowerbound = JsonConvert.SerializeObject(result.Milk_Production_LB);
            var prediction = JsonConvert.SerializeObject(result.Milk_Production);
            var month_dates = JsonConvert.SerializeObject(months);


            ViewBag.months = month_dates;
            ViewBag.lowerbound = lowerbound;
            ViewBag.prediction = prediction;
            ViewBag.upperbound = upperbound;
            ViewBag.extra = result.Milk_Production;


            float milk = _monthly_productionRepository.GetThisMonthData();
            ViewBag.thisMonth = milk;


            List<int> WeekWise = _productionRepository.GetWeekWiseData();
            
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Week" + i + " = " + WeekWise[i]);
            }


            var Weekly = JsonConvert.SerializeObject(WeekWise);
            ViewBag.weekly = Weekly;


            return View();
        }

        public IActionResult Login()
        {
            return View();

        }
    }




}