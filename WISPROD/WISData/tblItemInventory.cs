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
    
    public partial class tblItemInventory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblItemInventory()
        {
            this.tblInvoiceDetails = new HashSet<tblInvoiceDetail>();
            this.tblEstimateDetails = new HashSet<tblEstimateDetail>();
        }
    
        public int ItemInventoryID { get; set; }
        public string ItemInventoryNumber { get; set; }
        public string ItemInventoryDescription { get; set; }
        public Nullable<int> ItemCategoryID { get; set; }
        public Nullable<int> ItemInventoryQuantity { get; set; }
        public Nullable<int> LocationID { get; set; }
        public Nullable<int> ItemInventoryReorderPoint { get; set; }
        public Nullable<decimal> ItemInventoryReplacementCost { get; set; }
        public decimal ItemInventoryMarkup { get; set; }
        public Nullable<decimal> ItemInventorySalesPrice { get; set; }
        public Nullable<decimal> ItemInventoryReplacementCostOnHand { get; set; }
        public Nullable<decimal> ItemInventoryExtendedCost { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        public Nullable<System.DateTime> ModDate { get; set; }
        public string ModUser { get; set; }
        public bool Active { get; set; }
        public Nullable<decimal> ItemInventorySalesPriceOverride { get; set; }
        public string ItemInventoryPartNumber { get; set; }
        public Nullable<int> ManufacturerID { get; set; }
    
        public virtual tblLocation tblLocation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblInvoiceDetail> tblInvoiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblEstimateDetail> tblEstimateDetails { get; set; }
    }
}
