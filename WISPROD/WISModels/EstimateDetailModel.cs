using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
   public class EstimateDetailModel
    {
        public int EstimateDetailID { get; set; }
        public int EstimateID { get; set; }
        public int SortOrder { get; set; }
        public Nullable<int> ItemInventoryID { get; set; }
        public string EstimateDescription { get; set; }
        public string NonInventoryItem { get; set; }
        public string Categories { get; set; }
        public string Inventory { get; set; }
        public Nullable<int> ItemCategoryID { get; set; }
        public int LocID { get; set; }
        public string EstimateDetailCostCodeGL { get; set; }
        public string EstimateDetailCostCodeNonLabor { get; set; }
        public Nullable<decimal> ItemInventoryCost { get; set; }
        public Nullable<int> EstimateDetailQuantity { get; set; }
        public Nullable<decimal> EstimateDetailLineItemTotal { get; set; }

      
    }
}
