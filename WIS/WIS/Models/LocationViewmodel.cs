using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WIS.Models
{
    public class LocationViewmodel
    {
        public IEnumerable<SelectListItem> locationlist { get; set; }
        public int LocationID { get; set; }
        public string LocationDescription { get; set; }
    }
}