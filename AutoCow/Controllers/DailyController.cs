using AutoCow.Models;
using AutoCow.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

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
                Body_temperature = model.temperature,
                Breed_type = type,
                Milk_production = model.milk,
                Respiratory_rate = model.respiratory_rate,
                Heart_rate = model.heart_rate,
            };

            //Load model and predict output
            var result = MLModel.Predict(sampleData);
            Console.WriteLine(result.PredictedLabel);
            model.condition = result.PredictedLabel;
            model.type = type;
            Cow_profile cow_Profile;

            Console.WriteLine("Info recieved at controller now sending it to Repository");

            if(_daily_planRepository.Add(model) && _daily_planRepository.AddProduction(model))
            {
              cow_Profile =   _daily_planRepository.UpdateCow_ProfileData(model);
            }

            
            

            // Perform further processing or store the data in the database

            return RedirectToAction("Index", "Home"); // Redirect to a success page
        }






        [Route("Daily/Sensor")]
        [HttpGet]
        public ActionResult Sensor()
        {
            Daily_plan model = new Daily_plan();
         model.id = 12;
             model.type = _daily_planRepository.getTypeById(model.id);
         model.temperature = 39;
         model.milk = 20;
         model.respiratory_rate = 20;
         model.heart_rate = 65;
            model.disease = "N";
            model.insemination = "N";
           
            var sampleData = new MLModel.ModelInput()
            {
                Body_temperature = model.temperature,
                Breed_type = model.type,
                Milk_production = model.milk,
                Respiratory_rate = model.respiratory_rate,
                Heart_rate = model.heart_rate,
            };

            //Load model and predict output
            var result = MLModel.Predict(sampleData);
            Console.WriteLine(result.PredictedLabel);
            model.condition = result.PredictedLabel;

            var jsonString1 = JsonConvert.SerializeObject(model);
            Console.WriteLine(jsonString1);

            
            Daily_plan daily_Plan = JsonConvert.DeserializeObject<Daily_plan>(jsonString1);
            Console.WriteLine(daily_Plan.id + " " + daily_Plan.type +" " +daily_Plan.milk);

            if (_daily_planRepository.Add(daily_Plan) && _daily_planRepository.AddProduction(daily_Plan))
            {
                 _daily_planRepository.UpdateCow_ProfileData(daily_Plan);
            }


            return RedirectToAction("Index");
        }

    }
}
