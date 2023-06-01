using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoCow.Controllers
{
    public class DailyController : Controller
    {
        private readonly Daily_planRepository _daily_planRepository;

        public DailyController() {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _daily_planRepository = new Daily_planRepository(connectionString);

        }



        
        [Route("Daily/Index")]
        [HttpGet]
        public ActionResult Index()
        {
            return View(new Daily_plan());
        }

        [Route("Daily/Index")]
        [HttpPost]
        public ActionResult Index(Daily_plan model)
        {
            string type  = _daily_planRepository.getTypeById(model.id);

            //Load sample data
            var sampleData = new MLModel.ModelInput()
            {
                Id = model.id,
                Date = DateTime.Now,
                Milk = model.milk,
                Temperature = model.temperature,
                Insemination = model.insemination,
                Disease = model.disease,
                Type = type,
            };

            //Load model and predict output
            var result = MLModel.Predict(sampleData);
            Console.WriteLine(result.PredictedLabel);

            model.condition = result.PredictedLabel;
            model.type = type;


            Console.WriteLine("Info recieved at controller now sending it to Repository");

            _daily_planRepository.Add(model);

            _daily_planRepository.AddProduction(model);

            // Perform further processing or store the data in the database

            return RedirectToAction("Index"); // Redirect to a success page
        }

        [Route("Daily/Index")]
        public ActionResult Success()
        {
            return View();
        }

    }
}
