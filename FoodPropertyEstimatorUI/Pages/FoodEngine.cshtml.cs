using FoodPropertyEstimatorUI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace FoodPropertyEstimatorUI.Pages
{
    public class FoodEngineModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public List<Food> FoundationFoodList { get; set; }

        public List<Food> SRLegacyFoodList { get; set; }

        public List<Food> SurveyFoodList { get; set; }

        public List<Food> BrandedFoodList { get; set; }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.nal.usda.gov/fdc/");
                    //GET Method
                    var result = await client.GetAsync(
                                    "v1/foods/search?api_key=lsCrEwyvtgaxNhBgULvmtoJo0if8YD4rJjGeNQ5E&query=" + SearchString)
                                    .ConfigureAwait(false);
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<FoodsSearch>();
                        FoundationFoodList = readTask.Result.Foods.Where(x => x.DataType == "Foundation").ToList();
                        SRLegacyFoodList = readTask.Result.Foods.Where(x => x.DataType == "SR Legacy").ToList();
                        SurveyFoodList = readTask.Result.Foods.Where(x => x.DataType == "Survey (FNDDS)").ToList();
                        BrandedFoodList = readTask.Result.Foods.Where(x => x.DataType == "Branded").ToList();
                    }
                }
            }
            
            return Page();
        }
    }
}
