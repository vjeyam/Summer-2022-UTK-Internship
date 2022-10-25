using System.ComponentModel;

namespace FoodPropertyEstimatorUI.Model
{
    public class Food
    {
        public int FDCId { get; set; }

        [DisplayName("NDB Number")]
        public int NDBNumber { get; set; }

        public string Description { get; set; }

        [DisplayName("SR Food Category")]
        public string FoodCategory { get; set; }
    
        public DateTime AcquisitionDate { get; set; }

        public string DataType { get; set; }

        public int FoodCode { get; set; }

        public string AdditionalDescriptions { get; set; }

        public string GtinUpc { get; set; }

        public string BrandOwner { get; set; }
        
        public string BrandName { get; set; }
        
        public string MarketCountry { get; set; }
    }
}
