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
    
    public partial class spGetArchivedInvoice_Result
    {
        public int InvoiceID { get; set; }
        public string Contact { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public Nullable<int> ExternalClientID { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public string InvoiceJobNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceStatus { get; set; }
        public string InvoiceTerms { get; set; }
        public Nullable<decimal> InvoiceTotal { get; set; }
        public string InvoiceNotes { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        public Nullable<int> IsDeleted { get; set; }
        public Nullable<int> ClientTypeID { get; set; }
        public Nullable<int> LocationID { get; set; }
        public string Shipment { get; set; }
        public string Vendor_ { get; set; }
        public string VendorInvoice_ { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
    }
}
