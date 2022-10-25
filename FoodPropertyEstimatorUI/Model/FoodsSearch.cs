namespace FoodPropertyEstimatorUI.Model
{
    public class FoodsSearch
    {
        public int TotalHits { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<int> PageList { get; set; }
        public List<Food> Foods { get; set; }
    }
}
