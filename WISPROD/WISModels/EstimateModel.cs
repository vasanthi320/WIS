using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
    public class EstimateModel
    {
        public int Estimate { get; set; }
        public string EstimateNumber { get; set; }
        public Nullable<int> ClientTypeID { get; set; }
        [Required(ErrorMessage = "InvoiceJobNumber is Required")]
        public string client { get; set; }
        public string Description { get; set; }       

        public Nullable<decimal> Total { get; set; }
        public string Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedUser { get; set; }

        public List<EstimateDetailModel> EstimateDetails { get; set; }     
       
     
        [Required(ErrorMessage = "InvoiceJobNumber is Required")]
        public string InvoiceJobNumber { get; set; }      
        public string Vendor { get; set; }
        public string EstimateVendor { get; set; }
       
        public string EstimateNotes { get; set; }
       
        [Required(ErrorMessage = "Select the Employee")]
        public string Employee { get; set; }
        public string Project { get; set; }    
        public List<EmployeeModel> empdetails { get; set; }
        public List<LocationModel> Locationdetails { get; set; }
        public List<DefaultModel> defaultdetails { get; set; }
        public string DefaultValue { get; set; }
        public int LocationID { get; set; }
        public System.DateTime EstimateDate { get; set; }
        public int employID { get; set; }
        public string Location { get; set; }    
        public virtual IEnumerable<LocationModel> locationlist { get; set; }
        public Nullable<int> ExternalClientID { get; set; }
        public string ExernalClientName { get; set; }
        public List<ExternalClientModel> externaldtls { get; set; }
        public List<JobModel> JobDetails { get; set; }
        public String Contact { get; set; }
        public bool IsArchived { get; set; }
    }
    public class EstimateResult
    {
        public int EstimateId { get; set; }

        public bool Status { get; set; }
        public string Inventory { get; set; }
    }
}
