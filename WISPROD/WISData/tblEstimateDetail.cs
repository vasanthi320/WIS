//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WISData
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblEstimateDetail
    {
        public Nullable<int> SortOrder { get; set; }
        public int EstimateDetailID { get; set; }
        public int EstimateID { get; set; }
        public Nullable<int> ItemInventoryID { get; set; }
        public string EstimateDescription { get; set; }
        public string NonInventoryItem { get; set; }
        public string EstimateDetailCostCodeGL { get; set; }
        public Nullable<decimal> ItemInventoryCost { get; set; }
        public Nullable<int> EstimateDetailQuantity { get; set; }
        public Nullable<decimal> EstimateDetailLineItemTotal { get; set; }
        public string EstimateDetailCostCodeNonLabor { get; set; }
    
        public virtual tblEstimate tblEstimate { get; set; }
        public virtual tblItemInventory tblItemInventory { get; set; }
    }
}
