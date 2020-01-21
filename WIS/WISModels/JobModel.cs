using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WISModels
{
   public class JobModel
    {       
        public byte JCCo { get; set; }
        public string Job { get; set; }
        public string JobDescription { get; set; }
        public string MailAddress { get; set; }
        public string MailCity { get; set; }
        public string MailState { get; set; }
        public string MailZip { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string ShipState { get; set; }
        public string ShipZip { get; set; }
        public bool Active { get; set; }
    }
}
