using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WISModels;
using WISServiceLayer;

namespace WIS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Reports()
        {
            var service = new WISService();
            List<InvoiceModel> Iim = new List<InvoiceModel>();
            var model = service.GetInvoiceLists();
           // var lst = service.getInvoiceDashboarddata();            
            var invcmontotal = model.Select(i => i.InvoiceTotal).Sum();
            foreach (var item in model)
            {
                var email = GetUserEmail(item.CreatedUser);
                var name = GetUserFullname(item.CreatedUser);
                item.CreatedUser = name;
                item.TotalAmount = invcmontotal.Value;                
            }
            return View(model);
        }

        public ActionResult GetDashboardData()
        {
            var service = new WISService();
            var lst = service.getInvoiceDashboarddata();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        //public int TotalNumberOfDaysInMonth(int year, int month)
        //{
        //   var days= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        //    return days;
        //}
        public ActionResult GetInvoiceMonthSummary()
        {
            var emodel = new InvoiceSummary();
            var service = new WISService();
            var model = service.getInSummary(emodel);
            return View("_InvoiceMonthSummary",model);
        }
        public static string GetUserEmail(string userName)
        {
            var info = UserUtils.FindUserInfo(userName);
            info.Email = Utils.FixMailAddress(info.Email, "hoar.com");
            return info.Email;
        }
        public static string GetUserFullname(string userName)
        {
            var info = UserUtils.FindUserInfo(userName.Trim());
            return info.DisplayName;
        }
        public ActionResult Summary()
        {
            var service = new WISService();
            var model = service.GetInvoiceLists();
            var invcmontotal = model.Select(i=>i.InvoiceTotal).Sum();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}