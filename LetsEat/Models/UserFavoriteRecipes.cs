using System;
using System.Collections.Generic;

namespace LetsEat.Models
{
    public partial class UserFavoriteRecipes
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }

        public virtual FavoriteRecipes Recipe { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
