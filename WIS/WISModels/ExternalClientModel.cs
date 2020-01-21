using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
   public class ExternalClientModel
    {
        public int ExternalClientID { get; set; }
        [Required(ErrorMessage = "FirstName is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Employer is Required")]
        public string Employer { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Display(Name = "Province/State")]
        [RegularExpression("[A-Z]{2}")]
        public string State { get; set; }
        [Display(Name = "Postal/Zip Code")]
        public string Zip { get; set; }
    }
}
