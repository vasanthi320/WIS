using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public ActionResult GetBaiduGraphData(decimal InvoiceCompTotl,string Name)
        {
            var service = new WISService();
            string yr = Name.Substring(Name.Length - 4);
            string month = Name.Substring(0, Name.Length - 5);
            int Invoicemonth= DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;
            var model = service.GetGLInvoiceSummaryByMonth(Invoicemonth,Convert.ToInt32(yr));
            foreach (var item in model)
            {

            }
             return View(model);          
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
        public ActionResult GlCodesData()
        {
            string GlCode = "";
            var service = new WISService();
            List<InvoiceDetailModel> GlLst = new List<InvoiceDetailModel>();           
            var Emodel = service.GetGL1InvoiceSummary(GlCode);
            var Emodel1 = service.GetGL2InvoiceSummary(GlCode);
            ViewBag.GL1Total = Emodel.Select(i => i.InvoiceDetailLineItemTotal).Sum();
            ViewBag.GL2Total = Emodel1.Select(i => i.InvoiceDetailLineItemTotal).Sum();           
            return View("GlCodesData", Emodel);
        }
      
        public ActionResult GetGL1CodeInvoiceSummary()
        {
            string GlCode = "";
            var service = new WISService();
            List<InvoiceModel> GlLst = new List<InvoiceModel>();
            ViewBag.GLDeatils = "GL# CW29200.000.000070";
            var model = service.GetGL1InvoiceSummary(GlCode);
            return View("_InvoiceGLSummary", model);
        }
        public ActionResult GetGL2CodeInvoiceSummary()
        {
            string GlCode = "";
            var service = new WISService();
            List<InvoiceModel> GlLst = new List<InvoiceModel>();
            ViewBag.GLDeatils = "BM29200.000.000050";
            var model = service.GetGL2InvoiceSummary(GlCode);
            return View("_InvoiceGLSummary", model);
        }
        public ActionResult GetCompletedInvoiceSummary()
        {           
            var service = new WISService();
            List<InvoiceModel> GlLst = new List<InvoiceModel>();
            ViewBag.GLDeatils = "Completed";
            var model = service.GetCompletedInvoiceList();
            return View("_InvoiceGLSummary", model);
        }
    }
}