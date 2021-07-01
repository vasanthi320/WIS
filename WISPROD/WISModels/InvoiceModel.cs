using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WISModels
{
  public  class InvoiceModel
    {
        public int InvoiceID { get; set; }
        public string InvoiceNumber { get; set; }
        public System.DateTime InvoiceDate { get; set; }
        public Nullable<int> ClientTypeID { get; set; }
        public string ClientName { get; set; }
        [Required(ErrorMessage = "InvoiceJobNumber is Required")]
        public string InvoiceJobNumber { get; set; }        
        //public string InvoicePONumber { get; set; }
        public string Vendor { get; set; }
        public string InvoiceVendor { get; set; }
        public Nullable<decimal> InvoiceTotal { get; set; }       
        //public string InvoiceTerms { get; set; }       
        public string InvoiceNotes { get; set; }
        public string InvoiceStatus { get; set; }
        [Required(ErrorMessage = "Select the Employee")]
        public string Employee { get; set; }
       
        public string Project { get; set; }
        //public List<InvoiceModel> InvoiceModelDtls { get; set; }
        public List<InvoiceDetailModel> InvoiceDetails { get; set; }
        public List<ExternalClientModel> ExternalDetails { get; set; }
        public List<EmployeeModel> empdetails { get; set; }
        public List<LocationModel> Locationdetails { get; set; }
        public List<DefaultModel> defaultdetails { get; set; }
        public string DefaultValue { get; set; }
        public string DefaultCatValue { get; set; }
        public string DefaultItemCatIncludes { get; set; }
        public int LocationID { get; set; }
        public int employID { get; set; }
        public string Location { get; set; }
        public virtual IEnumerable<LocationModel> locationlist { get; set; }
        public Nullable<int> ExternalClientID { get; set; }
        public string ExternalClientFirstName { get; set; }
        public string ExternalClientLastName { get; set; }
        public List<JobModel> JobDetails { get; set; }

        public Nullable<decimal> TotalAmount { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }
        //public string InvoiceOrderedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public bool IsArchived { get; set; }
        public string Contact { get; set; }
        public int IsDeleted { get; set; }
        public string Shipment { get; set; }        
        public Nullable<decimal> InvoiceDetailLineItemTotal { get; set; }       
        public string InvoiceDetailCostCodeGL { get; set; }
        public string InvoiceDetailCostCodeNonLabor { get; set; }
        public int InvoiceDetailID { get; set; }
        public List<InvoicegData> InvoiceData { get; set; }
        public Decimal Completed { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName
        {
            get
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month);
            }
        }
    }
    public class InvoicegData
    {
        public string MonthName
        {
            get
            {
                return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Month).Substring(0,3);
            }
        }
        public Decimal Drafted { get; set; }
        public Decimal Completed { get; set; }
        public Decimal Archived { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

    }
    public class InvoiceResult
    {
        public int InvoiceId { get; set; }

        public bool Status { get; set; }
        public string Inventory { get; set; }
    }
}
