using LetsEat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEat.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecipeDAL _dal;

        public HomeController(RecipeDAL dal)
        {
            _dal = dal;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchResults(string DishName, string Ingredients, int Page)
        {
            TempData["DishName"] = DishName;
            TempData["Ingredients"] = Ingredients;
            TempData["QueryDescription"] = BuildQueryDescription(DishName, Ingredients);
            TempData["Page"] = Page;
            Rootobject ro;

            if (DishName == null)
            {
                ro = await _dal.SeachByIngredientsAsync(Ingredients, Page);
                
            }
            else if (Ingredients == null)
            {
                ro = await _dal.FindRecipesAsync(DishName, Page);
            }
            else
            {
                ro = await _dal.FindRecipesAsync(DishName, Ingredients, Page);
            }

            Result[] results = ro.results;

            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string BuildQueryDescription(string DishName, string Ingredients)
        {
            string queryDescription = "";

            if (DishName == null)
            {
                queryDescription = $"Recipes containing: '{Ingredients}'";
            }
            else if (Ingredients == null)
            {
                queryDescription = $"Recipes matching: '{DishName}'";
            }
            else
            {
                queryDescription = $"Recipes matching: '{DishName}' containing: '{Ingredients}'";
            }

            return queryDescription;
        }
    }
}
