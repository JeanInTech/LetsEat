using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LetsEat.Models
{
    public class RecipeDAL
    {
        private readonly HttpClient _client;

        public RecipeDAL(HttpClient client)
        {
            _client = client;
        }

        public async Task<Rootobject> FindRecipesAsync(string name, int page)
        {
            var response = await _client.GetAsync($"?q={ name }&p={ page }");
            Rootobject ro = new Rootobject();
            if (response.IsSuccessStatusCode)
            {
                string jsonData = await response.Content.ReadAsStringAsync();
                ro = JsonSerializer.Deserialize<Rootobject>(jsonData);
            }
            // if success status code is false it will increment the page number by 1 (found issue where some search pages returned 404 even with pages after ex: q=burger&p=2)
            else
            {
                response = await _client.GetAsync($"?q={ name }&p={ page + 1 }");
                string jsonData = await response.Content.ReadAsStringAsync();
                ro = JsonSerializer.Deserialize<Rootobject>(jsonData);
            }
            
            return ro;
        }

        public async Task<Rootobject> FindRecipesAsync(string name, string ingredients, int page)
        {
            var response = await _client.GetAsync($"?q={ name }&i={ ingredients }&p={ page }");
            string jsonData = await response.Content.ReadAsStringAsync();

            Rootobject ro = JsonSerializer.Deserialize<Rootobject>(jsonData);

            return ro;
        }


        public async Task<Rootobject> SeachByIngredientsAsync(string ingredients, int page)

        {
            var response = await _client.GetAsync($"?i={ ingredients }&p={ page }");
            string jsonData = await response.Content.ReadAsStringAsync();

            Rootobject ro = JsonSerializer.Deserialize<Rootobject>(jsonData);

            return ro;
        }
    }
}

