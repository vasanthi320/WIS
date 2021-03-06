using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
   public class InvoiceDetailModel
    {
        public int InvoiceDetailID { get; set; }
        public int InvoiceID { get; set; }
        public int InvSortOrder { get; set; }        
        public Nullable<int> ItemInventoryID { get; set; }
        public string ItemInventoryNumber { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string ItemInventoryDescription { get; set; }
        public Nullable<decimal> ItemInventoryUnitCost { get; set; }
        [Required(ErrorMessage = "Quantity is Required")]
        public Nullable<int> InvoiceDetailQuantity { get; set; }
        public Nullable<decimal> InvoiceDetailLineItemTotal { get; set; }
        [Required(ErrorMessage = "CostCode/GLCode is Required")]
        public string InvoiceDetailCostCodeGL { get; set; }
        public string InvoiceDetailCostCodeNonLabor { get; set; }
        public Nullable<int> ItemCategoryID { get; set; }
        public int LocID { get; set; }
        public string Categories { get; set; }
        public string Inventory { get; set; }
        public string NonInventoryItem { get; set; }
    }
}
