using FoodPropertyEstimatorUI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FoodPropertyEstimatorUI.Pages
{
    public class FoodDetailsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int FDCId { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public double Water { get; set; }

        public double BoundWater { get; set; }

        public double Ice { get; set; }

        [BindProperty]
        public double Fat { get; set; }

        [BindProperty]
        public double Carbohydrate { get; set; }

        [BindProperty]
        public double Protein { get; set; }

        [BindProperty]
        public double Ash { get; set; }

        [BindProperty]
        public double Fiber { get; set; }

        [BindProperty]
        public double Total { get; set; }

        public double TotalV { get; set; }

        public double SumV { get; set; }
        
        public double XvP { get; set; }

        public double XvFat { get; set; }

        public double XvC { get; set; }

        public double XvS { get; set; }

        public double XvM { get; set; }

        public double XvB { get; set; }

        public double XvI { get; set; }

        public double XvFib { get; set; }

        [BindProperty]
        public double Temperature { get; set; }

        public double ThermalConductivity { get; set; }

        public double CtcP { get; set; }

        public double CtcFat { get; set; }

        public double CtcC { get; set; }

        public double CtcS { get; set; }

        public double CtcM { get; set; }

        public double CtcB { get; set; }

        public double CtcI { get; set; }

        public double CtcFib { get; set; }

        public double CtcTot { get; set; }

        public double Density { get; set; }

        public double RhoP { get; set; }

        public double RhoFat { get; set; }

        public double RhoC { get; set; }

        public double RhoS { get; set; }

        public double RhoM { get; set; }

        public double RhoB { get; set; }

        public double RhoI { get; set; }

        public double RhoFib { get; set; }

        public double RhoTot { get; set; }

        public double SpecificHeatCapacity { get; set; }

        public double CpP { get; set; }

        public double CpFat { get; set; }

        public double CpC { get; set; }

        public double CpS { get; set; }

        public double CpM { get; set; }

        public double CpB { get; set; }

        public double CpI { get; set; }

        public double CpFib { get; set; }

        public double CpTot { get; set; }

        public double ThermalDiffusivity { get; set; }

        public double AlphaP { get; set; }

        public double AlphaFat { get; set; }

        public double AlphaC { get; set; }

        public double AlphaS { get; set; }

        public double AlphaM { get; set; }

        public double AlphaB { get; set; }

        public double AlphaI { get; set; }

        public double AlphaFib { get; set; }

        public double AlphaTot { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            FDCId = id;
            
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

                    Description = readTask.Result.Description;

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

            CalculateBoundIce();

            Total = Water + Fat + Carbohydrate + Protein + Ash + Fiber;

            RoundFoodPropertiesValues();

            return Page();
        }

        public void OnPostCalculate()
        {
            CalculateBoundIce();
            
            // Partial Density
            RhoP = Math.Round(1329.9 - (0.5184 * Temperature), 4);
            RhoFat = Math.Round(925.59 - (0.41757 * Temperature), 4);
            RhoC = Math.Round(1599.1 - (0.31046 * Temperature), 4);
            RhoS = Math.Round(2423.8 - (0.28063 * Temperature), 4);
            RhoFib = Math.Round(1311.5 - 0.36589 * Temperature, 4);
            if (Temperature < 0)
            {
                RhoB = Math.Round(997.18 + (0.0031439 * Temperature) - (0.0037574 * Math.Pow(Temperature, 2.0)), 4);
                RhoI = Math.Round(916.89 - 0.13071 * Temperature);
                RhoM = Math.Round(997.18 + (0.0031439 * Temperature) - (0.0037574 * Math.Pow(Temperature, 2.0)), 4);
            }
            else
            {
                RhoM = Math.Round(997.18 + (0.0031439 * Temperature) - (0.0037574 * Math.Pow(Temperature, 2.0)), 4);
            }

            // Volume Fraction
            if (Temperature < 0)
            {
                SumV = ((Protein / RhoP) + (Fat / RhoFat) + (Carbohydrate / RhoC) + (Ash / RhoS) + (BoundWater / RhoB) + (Ice / RhoI) + (Fiber / RhoFib));
                XvP = Math.Round(((Protein / RhoP) / SumV), 4);
                XvFat = Math.Round(((Fat / RhoFat) / SumV), 4);
                XvC = Math.Round(((Carbohydrate / RhoC) / SumV), 4);
                XvS = Math.Round(((Ash / RhoS) / SumV), 4);
                XvFib = Math.Round(((Fiber / RhoFib) / SumV), 4);
                XvM = Math.Round(((Water / RhoM) / SumV), 4);
                XvB = Math.Round(((BoundWater / RhoB) / SumV), 4);
                XvI = Math.Round(((Ice / RhoI) / SumV), 4);
                TotalV = XvFat + XvC + XvS + XvFib + XvB + XvI + XvP;
            }
            else
            {
                SumV = ((Protein / RhoP) + (Fat / RhoFat) + (Carbohydrate / RhoC) + (Ash / RhoS) + (Water / RhoM) + (Fiber / RhoFib));
                XvP = Math.Round(((Protein / RhoP) / SumV), 4);
                XvFat = Math.Round(((Fat / RhoFat) / SumV), 4);
                XvC = Math.Round(((Carbohydrate / RhoC) / SumV), 4);
                XvS = Math.Round(((Ash / RhoS) / SumV), 4);
                XvM = Math.Round(((Water / RhoM) / SumV), 4);
                XvFib = Math.Round(((Fiber / RhoFib) / SumV), 4);
                TotalV = XvFat + XvC + XvS + XvFib + XvM + XvP;
            }
            
            // Partial Thermal Conductivity
            // CtcP
            CtcP = Math.Round(0.17781 + (0.0011958 * Temperature) - (0.0000027178 * Math.Pow(Temperature, 2.0)), 4);
            CtcFat = Math.Round(0.18071 - (0.0027604 * Temperature) - (0.00000017749 * Math.Pow(Temperature, 2.0)), 4);
            CtcC = Math.Round(0.20141 + (0.0013874 * Temperature) - (0.0000043312 * Math.Pow(Temperature, 2.0)), 4);
            CtcS = Math.Round(0.32962 + (0.0014011 * Temperature) - (0.0000029069 * Math.Pow(Temperature, 2.0)), 4);
            CtcM = Math.Round(0.57109 + (0.0017625 * Temperature) - (0.0000067036 * Math.Pow(Temperature, 2.0)), 4);
            CtcFib = Math.Round(0.18331 + (0.0012497 * Temperature) - (0.0000031683 * Math.Pow(Temperature, 2.0)), 4);
            if (Temperature < 0)
            {
                CtcB = Math.Round(0.57109 + (0.0017625 * Temperature) - (0.0000067036 * Math.Pow(Temperature, 2.0)), 4);
                CtcI = Math.Round(2.2196 - 0.0062489 * Temperature + 0.00010154 * Math.Pow(Temperature, 2), 4);
            }

            // Partial Specific Heat
            CpP = Math.Round(2.0082 + (0.0012089 * Temperature) - (0.0000013129 * Math.Pow(Temperature, 2.0)), 4);
            CpFat = Math.Round(1.9842 + (0.0014733 * Temperature) - (0.0000048008 * Math.Pow(Temperature, 2.0)), 4);
            CpC = Math.Round(1.5488 + (0.0019625 * Temperature) - (0.0000059399 * Math.Pow(Temperature, 2.0)), 4);
            CpS = Math.Round(1.0926 + (0.0018896 * Temperature) - (0.0000036817 * Math.Pow(Temperature, 2.0)), 4);
            CpM = Math.Round(4.1762 - (0.000090864 * Temperature) + (0.0000054731 * Math.Pow(Temperature, 2.0)), 4);
            CpFib = Math.Round(1.8459 + 0.0018306 * Temperature - (0.0000046509 * Math.Pow(Temperature, 2.0)), 4);
            if (Temperature < 0)
            {
                CpB = Math.Round(4.1762 - (0.000090864 * Temperature) + (0.0000054731 * Math.Pow(Temperature, 2.0)), 4);
                CpI = Math.Round((2.0623 + 0.0060769 * Temperature), 4);
            }

            AlphaP = CtcP / (RhoP * CpP);
            AlphaFat = CtcFat / (RhoFat * CpFat);
            AlphaC = CtcC / (RhoC * CpC);
            AlphaS = CtcS / (RhoS * CpS);
            AlphaM = CtcM / (RhoM * CpM);
            AlphaFib = CtcFib / (RhoFib * CpFib);

            // Total Values
            RhoTot = Math.Round((1 / SumV), 2);
            Total = Math.Round(Total, 2);
            TotalV = Math.Round(TotalV, 2);
            if (Temperature >= 0)
            {
                CtcTot = Math.Round(((CtcP * XvP) + (CtcFat * XvFat) + (CtcC * XvC) + (CtcS * XvS) + (CtcM * XvM) + (CtcFib * Fiber)), 4);
                CpTot = Math.Round(((CpP * Protein) + (CpFat * Fat) + (CpC * Carbohydrate) + (CpS * Ash) + (CpM * Water) + (CpFib * Fiber)), 4);
            }
            else
            {
                CtcTot = Math.Round(((CtcP * XvP) + (CtcFat * XvFat) + (CtcC * XvC) + (CtcS * XvS) + (CtcB * XvB) + (CtcI * XvI) + (CtcFib * Fiber)), 4);
                CpTot = Math.Round(((CpP * Protein) + (CpFat * Fat) + (CpC * Carbohydrate) + (CpS * Ash) + (CpB * BoundWater) + (CpI * Ice) + (CpFib * Fiber)), 4);
            }

            AlphaTot = CtcTot / (RhoTot * CpTot * 1000);

            RoundFoodPropertiesValues();
        }

        private void RoundFoodPropertiesValues()
        {
            Water = Math.Round(Water, 4);
            Fat = Math.Round(Fat, 4);
            Carbohydrate = Math.Round(Carbohydrate, 4);
            Ash = Math.Round(Ash, 4);
            Protein = Math.Round(Protein, 4);
            Fiber = Math.Round(Fiber, 4);
            Total = Math.Round(Total, 2);
        }

        private void CalculateBoundIce()
        {
            if (Temperature < 0)
            {
                BoundWater = Protein * 0.4;
                if (BoundWater > Water)
                {
                    BoundWater = Water;
                }

                Ice = Water - BoundWater;
            }
        }
    }
}
