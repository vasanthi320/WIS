using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
    public class ItemInventoryModel
    {
        public int ItemInventoryID { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public string ItemInventoryNumber { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string ItemInventoryDescription { get; set; }       
        public Nullable<int> ItemCategoryID { get; set; }
        [Required(ErrorMessage = "Quantity should be greater than zero")]
        public string CategoryDescription { get; set; }
        public Nullable<int> ItemInventoryQuantity { get; set; }
        [Required(ErrorMessage = "Select the Location")]
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> ItemInventoryReorderPoint { get; set; }
        public Nullable<decimal> ItemInventoryReplacementCost { get; set; }
        public Nullable<decimal> ItemInventoryMarkup { get; set; }
        public Nullable<decimal> ItemInventorySalesPrice { get; set; }
        public Nullable<decimal> ItemInventorySalesPriceOverride { get; set; }
        public Nullable<decimal> ItemInventoryReplacementCostOnHand { get; set; }
        public Nullable<decimal> ItemInventoryExtendedCost { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public System.DateTime? ModDate { get; set; }
        public string ModUser { get; set; }
        public string LocationDescription { get; set; }
        public bool Active { get; set; }
        public List<LocationModel> LocationDetails { get; set; }
        public List<ItemCategory> ItemCategories { get; set; }
        public string DefaultCatValue { get; set; }        
        public string DefaultItemCatIncludes { get; set; }
    }
}
