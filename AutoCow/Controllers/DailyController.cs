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
        private readonly Cow_ProfileRepository _cowProfileRepository;

        public DailyController() {
            string connectionString = "Data Source=localhost;Initial Catalog=milk;Integrated Security=True;";
            _daily_planRepository = new Daily_planRepository(connectionString);
            _cowProfileRepository = new Cow_ProfileRepository(connectionString);

        }



        
        [Route("Daily/Index")]
        [HttpGet]
        public ActionResult Index()
        {
            List<Cow_profile> cow = _cowProfileRepository.GetAllCows();
            var ids = cow.Select(d => d.id).ToArray();

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
            Cow_profile cow_Profile = _cowProfileRepository.GetCowById(model.id);


            if (model.symptom1 != null) { 
            //Load sample data to predict disease
            var sampleData1 = new MLModel2.ModelInput()
            {
                Animal = @"cow",
                Age = cow_Profile.age,
                Temperature = model.temperature,
                Symptom_1 = model.symptom1,
                Symptom_2 = model.symptom2,
                Symptom_3 = model.symptom3,
            };

            //Load model and predict output
            var disease = MLModel2.Predict(sampleData1);
            model.disease = disease.PredictedLabel;

            }
            else
            {
                model.disease = "N";
            }
            Console.WriteLine("Info recieved at controller now sending it to Repository");

            if(_daily_planRepository.Add(model) && _daily_planRepository.AddProduction(model))
            {
               _daily_planRepository.UpdateCow_ProfileData(model);
            }

            _daily_planRepository.AddMonthlyProduction(model);
            
            

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
