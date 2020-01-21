using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
    public class InvoiceSummary
    {
        public decimal InvoiceSummaryTotal0To30 { get; set; }
        public decimal InvoiceSummaryTotal31To60 { get; set; }
        public decimal InvoiceSummaryTotal61To90 { get; set; }
        public decimal InvoiceSummaryTotal91Plus { get; set; }
        public decimal InvoiceSummaryTotalAll { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    }
}
