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
            var recipes = from r in _db.FavoriteRecipes
                          where _db.UserFavoriteRecipes.Any(x => x.UserId == user && x.RecipeId == r.Id)
                          select r;
            List<FavoriteRecipes> RecipeList = recipes.ToList();

            return View(RecipeList);
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(string title, string href, string ingredients, string thumbnail)
        {
            FavoriteRecipes r = new FavoriteRecipes();
            r.Title = title;
            r.RecipeUrl = href;
            r.Ingredients = ingredients;
            r.Thumbnail = thumbnail;

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
        [HttpGet]
        public IActionResult UpdateFavorite(int Id)
        {
            FavoriteRecipes r = _db.FavoriteRecipes.Find(Id);
            return View(r);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFavorite(FavoriteRecipes r)
        {
            if (ModelState.IsValid)
            {
                _db.FavoriteRecipes.Update(r);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("ShowAllFavorites");
        }
        [HttpGet]
        public IActionResult DeleteFavorite(int Id)
        {
            FavoriteRecipes r = _db.FavoriteRecipes.Find(Id);

            return View(r);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFavorite(FavoriteRecipes r)
        {
            var recipe = _db.UserFavoriteRecipes.Where(x => x.RecipeId == r.Id).First();
            _db.UserFavoriteRecipes.Remove(recipe);

            _db.FavoriteRecipes.Remove(r);
            await _db.SaveChangesAsync();

            return RedirectToAction("ShowAllFavorites");
        }
        public IActionResult RandomFavorite()
        {
            Guid g = Guid.NewGuid();
            var user = FindUser();
            var recipes = from r in _db.FavoriteRecipes
                          where _db.UserFavoriteRecipes.Any(x => x.UserId == user && x.RecipeId == r.Id)
                          select r;

            var randomRecipe = recipes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

            return View(randomRecipe);
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
