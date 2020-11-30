using LetsEat.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LetsEat.Controllers
{
    [Authorize]
    public class LetsEatController : Controller
    {
        private readonly LetsEatContext _db; 
        public LetsEatController(LetsEatContext db)
        {
            _db = db;
        }
        public IActionResult ShowAllFavorites()
        {
            var user = FindUser();
            var recipes = from all in _db.FavoriteRecipes
                          where _db.UserFavoriteRecipes.Any(x => x.UserId == user)
                          select all;
            List<FavoriteRecipes> RecipeList = recipes.ToList();

            return View(RecipeList);
        }
        [HttpGet]
        public IActionResult GetRecipe(Result r)
        {
            TempData["Name"] = r.title;

            return View(r);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(FavoriteRecipes r)
        {
            if (ModelState.IsValid)
            {
                await _db.FavoriteRecipes.AddAsync(r);
                await _db.SaveChangesAsync();

                var user = FindUser();
                UserFavoriteRecipes f = new UserFavoriteRecipes(user, r.Id);

                await _db.UserFavoriteRecipes.AddAsync(f);
                await _db.SaveChangesAsync();

                return RedirectToAction("ShowAllFavorites");
            }
            return View("GetRecipe");
        }

        public string FindUser()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            return userId;
        }
    }
}
