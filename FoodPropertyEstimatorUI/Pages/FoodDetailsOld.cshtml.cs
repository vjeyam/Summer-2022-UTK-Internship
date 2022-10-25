using FoodPropertyEstimatorUI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodPropertyEstimatorUI.Pages
{
    public class FoodDetailsModelOld : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int FDCId { get; set; }
        
        public double Water { get; set; }

        public double Fat { get; set; }

        public double Carbohydrate { get; set; }

        public double Protein { get; set; }

        public double Ash { get; set; }

        public double Fiber { get; set; }

        public double Total { get; set; }

        public double Temperature { get; set; }

        public double ThermalConductivity { get; set; }

        public double Density { get; set; }

        public double SpecificHeatCapacity { get; set; }

        public double ThermalDiffusivity { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.nal.usda.gov/fdc/");
                //GET Method
                var result = await client.GetAsync(
                                "v1/food/" + id + "?format=abridged&nutrients=203&nutrients=204&nutrients=205&nutrients=255&nutrients=207&nutrients=291&api_key=lsCrEwyvtgaxNhBgULvmtoJo0if8YD4rJjGeNQ5E")
                               .ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<Nutrients>();

                    foreach (var nutrient in readTask.Result.FoodNutrients)
                    {
                        switch (nutrient.Number)
                        {
                            case "203":
                                Protein = nutrient.Amount;
                                break;

                            case "204":
                                Fat = nutrient.Amount;
                                break;

                            case "205":
                                Carbohydrate = nutrient.Amount;
                                break;

                            case "207":
                                Ash = nutrient.Amount;
                                break;

                            case "255":
                                Water = nutrient.Amount;
                                break;

                            case "291":
                                Fiber = nutrient.Amount;
                                break;
                        }
                    }
                }
            }

            Water = Water / 100;
            Fat = Fat / 100;
            Carbohydrate = (Carbohydrate - Fiber) / 100;
            Protein = Protein / 100;
            Ash = Ash / 100;
            Fiber = Fiber / 100;

            Total = Water + Fat + Carbohydrate + Protein + Ash + Fiber;
            if (Total > 1)
            {
                Water = Water / Total;
                Fat = Fat / Total;
                Carbohydrate = Carbohydrate / Total;
                Protein = Protein / Total;
                Fiber = Fiber / Total;
            }
            else
            {
                if (Water == 0)
                {
                    Water = 1 - Total;
                }
                else
                {
                    Ash = Ash + (1 - Total);
                }
            }

            return Page(); 
        }
    }
}
