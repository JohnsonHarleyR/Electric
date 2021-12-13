using Electric.UI.Helpers;
using Electric.UI.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Electric.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // test the level generator
            LevelGenerator generator = new LevelGenerator();
            LevelStageModel testLevel = generator.CreateRandomLevel();

            // test list of levels
            List<LevelStageModel> testLevelList = generator.GenerateLevels(50);

            return View(testLevel);
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