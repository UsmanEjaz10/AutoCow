using Microsoft.AspNetCore.Mvc;
using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Http;

namespace AutoCow.Controllers
{
    public class Cow_ProfileController : Controller
    {
        private readonly Cow_ProfileRepository _cow_profileRepository;

        public Cow_ProfileController()
        {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _cow_profileRepository = new Cow_ProfileRepository(connectionString);

        }


        [Route("Cow_Profile/Index")]
        public IActionResult Index()
        {
            List<Cow_profile> cow_ProfileList = _cow_profileRepository.GetAllCows();

            ViewBag.Cows = cow_ProfileList;

            return View();
        }

        
    }
}
