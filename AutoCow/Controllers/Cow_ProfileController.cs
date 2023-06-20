using Microsoft.AspNetCore.Mvc;
using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AutoCow.Controllers
{
    public class Cow_ProfileController : Controller
    {
        private readonly Cow_ProfileRepository _cow_profileRepository;
        private readonly Daily_planRepository _daily_planRepository;

        public Cow_ProfileController()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _cow_profileRepository = new Cow_ProfileRepository(connectionString);
            _daily_planRepository = new Daily_planRepository(connectionString);

        }


        [Route("Cow_Profile/Index")]
        public ActionResult Index()
        {
            List<Cow_profile> cow_ProfileList = _cow_profileRepository.GetAllCows();

            ViewBag.Cows = cow_ProfileList;
            ViewBag.size = cow_ProfileList.Count;

            return View(new Cow_profile());
        }



        

        [Route("Cow_Profile/Add")]
        [HttpGet]
        public ActionResult AddCow()
        {
            return View(new Cow_profile());
        }

        [Route("Cow_Profile/Add")]
        [HttpPost]
        public ActionResult AddCow(Cow_profile profile)
        {
            _cow_profileRepository.AddCow(profile);
            return RedirectToAction("Index");
        }

        [Route("Cow_Profile/Profile")]

        public IActionResult Profile(int id) {

            Cow_profile cow_Profile = _cow_profileRepository.GetCowById(id);
            List<Daily_plan> plans = _daily_planRepository.GetDailyDataById(id);
            Daily_plan weekly_milk = _daily_planRepository.getTotalMilkWeekly(id);


            Console.WriteLine("Condition = " + plans[0].condition);

            ViewBag.profile = cow_Profile;
            ViewBag.plans = plans;
            ViewBag.weekly_milk = weekly_milk;



            var dates = plans.Select(d => d.date.ToShortDateString()).ToArray();
            var milk = plans.Select(d => d.milk).ToArray();
            var temperature = plans.Select(d => d.temperature).ToArray();


            Console.WriteLine(dates[0] + " " + milk[0] + " " + temperature[0]);

            ViewBag.dates = JsonConvert.SerializeObject(dates);
            ViewBag.milk = JsonConvert.SerializeObject(milk);
            ViewBag.temperature = JsonConvert.SerializeObject(temperature);

            return View();
        }

        [Route("Cow_Profile/Delete")]
        public IActionResult deleteCow(int id)
        {
            _cow_profileRepository.deleteCow(id);
            _daily_planRepository.deleteCowDailyData(id);

            return RedirectToAction("Index");
        }
    }
}
