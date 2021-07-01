using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
   public class EmployeeLocation
    {
        public int EmployeeID { get; set; }
        public int LocationID { get; set; }
        public string LocatonDescription { get; set; }
        public bool UseAllAsDefault { get; set; }
        public string email { get; set; }
    }
}
