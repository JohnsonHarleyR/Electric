using Electric.UI.Global;
using Electric.UI.Helpers;
using Electric.UI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Electric.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        [HttpPost]
        public string GetLevels()
        {
            // initiate the level generator
            LevelGenerator generator = new LevelGenerator();

            // Get list of levels
            List<LevelStageModel> levelList = generator.GenerateLevels(GlobalVariables.NUMBER_OF_LEVELS, GlobalVariables.MAX_STEPS);

            return JsonConvert.SerializeObject(levelList);
        }









        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}