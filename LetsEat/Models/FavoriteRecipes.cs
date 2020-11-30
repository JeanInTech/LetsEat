using System;
using System.Collections.Generic;

namespace LetsEat.Models
{
    public partial class FavoriteRecipes
    {
        public FavoriteRecipes()
        {
            UserFavoriteRecipes = new HashSet<UserFavoriteRecipes>();
        }

        public int Id { get; set; }
        public string RecipeUrl { get; set; }
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Thumbnail { get; set; }
        public byte? Rating { get; set; }
        public string Category { get; set; }

        public virtual ICollection<UserFavoriteRecipes> UserFavoriteRecipes { get; set; }
    }
}
