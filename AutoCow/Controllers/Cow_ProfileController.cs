using Microsoft.AspNetCore.Mvc;
using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Http;

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

            return View(new Cow_profile());
        }

        [Route("Cow_Profile/Index")]
        [HttpPost]
        public ActionResult Index(Cow_profile form)
        {
            Cow_profile profile = new Cow_profile();
            profile.id = Convert.ToInt32(form.id);
            profile.price = Convert.ToInt32(form.price);
            profile.type  = Convert.ToString(form.type);
            profile.color = Convert.ToString(form.color);
            profile.avg_milk = Convert.ToInt32(form.avg_milk);
            profile.avg_temperature = Convert.ToInt32(form.avg_temperature);
            profile.category = Convert.ToString(form.category);

            Console.WriteLine(profile.id + " " + profile.avg_milk);
         //   String action  = Convert.ToString(form["action"]);
         /*   if(action == "update")
            {
                ViewBag.cow_profile = profile;
                return RedirectToAction("Add");
            }
            else if(action == "delete")
            {
                _cow_profileRepository.deleteCow(profile);
                _daily_planRepository.deleteCowDailyData(profile);
            }
         */
            return View();
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


            return View();
        }
    }
}
