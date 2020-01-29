using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WISModels;
using WISServiceLayer;

namespace WIS.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        public ActionResult Invoices()
        {
            var service = new WISService();
            List<InvoiceModel> Iim = new List<InvoiceModel>();
            var model = service.GetInvoiceList();
            return View(model);
        }
        public JsonResult GetProjects(string term)
        {
            var service = new WISService();
            List<InvoiceModel> Iim = new List<InvoiceModel>();
            var Joblist = service.GetJobDetails();
            var jobName = Joblist.Where(n => (n.JobDescription).StartsWith(term));
            return Json(jobName, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories(string term)
        {
            var service = new WISService();
            List<InvoiceDetailModel> Iim = new List<InvoiceDetailModel>();
            var Catlist = service.GetItemCategories();
            var CategoryName = Catlist.Where(n => (n.ItemCategoryDescription).StartsWith(term))/*.OrderByDescending(p => p.ItemCategoryDescription).ToList()*/;
            return Json(CategoryName, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditInvoice(int InvoiceId)
        {
            var service = new WISService();
            HttpContext.Session["Invoice"] = null;
            //string loc = service.GetEmpLocation(GetUserEmail(User.Identity.Name));
            //loc = loc == "All" ? "Bessemer" : loc;
            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p => p.Email == useremail).SingleOrDefault().EmployeeID;
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp != null ? emp.LocationID : 1;

            var model = service.GetInvoiceDetails(InvoiceId);
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job.ToString() }).ToList();
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString() }).ToList();
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().
               Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString()}).ToList();
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            if (model.InvoiceID > 0)
            {
                // HttpContext.Session["Invoice"] = model;
                return View("editInvoice", model);
            }
            model.LocationID = Emplocation;
            return View("AddInvoiceItem", model);
        }
        public ActionResult AddnewInvoice(string mesg = "")
        {
            var service = new WISService();
            ViewBag.Result = mesg;
            //string loc = service.GetEmpLocation(GetUserEmail(User.Identity.Name));
           
           // loc = loc == "All" ? "Bessemer" : loc;                   
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString() }).ToList();

            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p=>p.Email==useremail).SingleOrDefault().EmployeeID;
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp !=null? emp.LocationID : 1;
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();

            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
           
            
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            var model = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();
            model.InvoiceDetails = model.InvoiceDetails ?? new List<InvoiceDetailModel>();            
            model.ClientTypeID = 2;
            model.LocationID = Emplocation;
            return View("AddInvoiceItem", model);
            
        }
     
        public SelectList ToSelectList(List<LocationModel> loclist)
        {
            var service = new WISService();
            List<SelectListItem> list = new List<SelectListItem>();
            var tblloc = service.GetLocationList();
            string loc = service.GetEmpLocation(GetUserEmail(User.Identity.Name));
            loc = loc == "All" ? "Bessemer" : loc;
            foreach (LocationModel p in loclist)
            {
                list.Add(new SelectListItem()
                {
                    Text = p.LocationDescription,
                    Value = Convert.ToString(p.LocationID)
                });
            }

            return new SelectList(list, "Value", "Text");
        }
        public ActionResult PopupAddInvoiceItem(int Location)
        {
            var service = new WISService();
            var model = new InvoiceDetailModel();
            //string loc = service.GetEmpLocation(GetUserEmail(User.Identity.Name)); 
            //ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0 && x.LocationID == Location).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() + "|" + p.ItemInventorySalesPrice + "|" + p.ItemInventoryQuantity }).ToList();
            var Inventorylst = new List<SelectListItem>();            
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });                    
            ViewBag.Inventory = Inventorylst;            
           ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
           
            return View("_popupInvoiceItem", model);
        }
        public ActionResult EditPopupAddInvoiceItem(int Location)
        {
            var service = new WISService();
            var model = new InvoiceDetailModel();
            var Inventorylst = new List<SelectListItem>();
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });
            ViewBag.Inventory = Inventorylst;
            //ViewBag.Location = service.GetLocationList().
            //    Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString(), Selected = p.LocationDescription == loc }).ToList();        
            //ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0 && x.LocationID == Location).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() + "|" + p.ItemInventorySalesPrice + "|" + p.ItemInventoryQuantity }).ToList();
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            // var item = new InvoiceDetailModel { InvoiceDetailID = -1 };
            return View("_editpopupInvoiceItem", model);
        }
        [HttpPost]
        public ActionResult AddInvoiceItems(int Id, int qty, decimal price, string CGCode, string desc, int cat, string catText,int locaton)
        {
            var lst = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();           
            if (lst.InvoiceDetails == null)
            {
                lst.InvoiceDetails = new List<InvoiceDetailModel>();
            }
            lst.InvoiceDetails.Add(new InvoiceDetailModel { InvoiceDetailID = lst.InvoiceDetails.Count, ItemInventoryDescription = desc, ItemInventoryID = Id, InvoiceDetailQuantity = qty, ItemInventoryUnitCost = price, InvoiceDetailLineItemTotal = qty * price, InvoiceDetailCostCodeGL = CGCode, Categories = catText, ItemCategoryID = cat });
            lst.InvoiceTotal = GetTotal(lst);
            ViewBag.Total = lst.InvoiceTotal;
            HttpContext.Session["Invoice"] = lst;
            lst.LocationID = locaton;
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        public ActionResult AddInvoiceItems1(int Id, int qty, decimal price, string CGCode, string itm1, int InvoiceId)
        {
            var lst = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();
            if (lst.InvoiceDetails == null)
            {
                lst.InvoiceDetails = new List<InvoiceDetailModel>();
            }
            lst.InvoiceDetails.Add(new InvoiceDetailModel { InvoiceDetailID = lst.InvoiceDetails.Count,  InvoiceDetailQuantity = qty, ItemInventoryUnitCost = price, InvoiceDetailLineItemTotal = qty * price, InvoiceDetailCostCodeGL = CGCode, NonInventoryItem = itm1 });
            lst.InvoiceTotal = GetTotal(lst);
            ViewBag.Total = lst.InvoiceTotal;
            HttpContext.Session["Invoice"] = lst;
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        public ActionResult EditAddInvoiceItems(int Id, int qty, decimal price, string CGCode, string desc, int cat, string catText,int InvoiceId)
        {
            var service = new WISService();
            bool sucess = service.SaveInvoiceItem(InvoiceId, new InvoiceDetailModel {  ItemInventoryDescription = desc, ItemInventoryID = Id, InvoiceDetailQuantity = qty, ItemInventoryUnitCost = price, InvoiceDetailLineItemTotal = qty * price, InvoiceDetailCostCodeGL = CGCode, Categories = catText, ItemCategoryID = cat });
            var lst = service.GetInvoiceDetails(InvoiceId);            
            lst.InvoiceTotal = GetTotal(lst);
            ViewBag.Total = lst.InvoiceTotal;          
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        public ActionResult EditAddInvoiceItems1(int Id, int qty, decimal price, string CGCode, string itm1, int InvoiceId)
        {
            var service = new WISService();
            bool sucess = service.SaveInvoiceItem(InvoiceId, new InvoiceDetailModel { ItemInventoryID = Id==0 ? (int?)null :Id, InvoiceDetailQuantity = qty, ItemInventoryUnitCost = price, InvoiceDetailLineItemTotal = qty * price, InvoiceDetailCostCodeGL = CGCode, NonInventoryItem = itm1 });
            var lst = service.GetInvoiceDetails(InvoiceId);
            lst.InvoiceTotal = GetTotal(lst);
            ViewBag.Total = lst.InvoiceTotal;
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        public ActionResult GetInvoiceItems()
        {
            var lst = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        private decimal GetTotal(InvoiceModel e)
        {
            decimal m = 0;
            if (e.InvoiceDetails != null && e.InvoiceDetails.Any()) { m = e.InvoiceDetails.Sum(p => p.InvoiceDetailLineItemTotal.Value); }
            var amt = Math.Round(m, 2);
            return amt;
        }
        //Save Invoice
        public ActionResult SaveInvoiceDetails(InvoiceModel model)
        {
            var Emodel = HttpContext.Session["Invoice"] as InvoiceModel;
            var service = new WISService();
            model.InvoiceDetails = Emodel.InvoiceDetails;
            Emodel.InvoiceTotal = GetTotal(Emodel);
            model.InvoiceTotal = Emodel.InvoiceTotal;
            model.CreatedUser = User.Identity.Name;
            var email = GetUserEmail(model.CreatedUser);
            var res = SaveInvoice(model);
            if (res.Status)
            {
                model.InvoiceID = res.InvoiceId;
                switch (model.InvoiceStatus)
                {

                    case "D": break;
                    case "A":
                        SendAcctInvoiceEmail(model, ConfigurationManager.AppSettings["AccountEmail"]);                       
                        break;
                    case "ME":
                        SendInvoiceEmail(model, email);
                        break;
                }
                HttpContext.Session["Invoice"] = null;
                return RedirectToAction("Invoices", "Invoice");
            }
            else
            {
                HttpContext.Session["Invoice"] = model;
                return RedirectToAction("AddnewInvoice", new { mesg = res.Inventory });
            }
        }
        //Save Edit Invoice
        public ActionResult SaveEditInvoiceDetails(InvoiceModel model)
        {           
            var service = new WISService();           
            model.CreatedUser = User.Identity.Name;
            var email = GetUserEmail(model.CreatedUser);
            //var res = bool.TryParse(model.InvoiceStatus, out bool result);
            var res =!string.IsNullOrEmpty( model.InvoiceStatus);
           // bool flag;
           // if (Boolean.TryParse(model.InvoiceStatus, out flag))
             if (res)
             {                          
                switch(model.InvoiceStatus)
                {
                    case "D": break;
                    case "A":
                        SendAcctInvoiceEmail(model, ConfigurationManager.AppSettings["AccountEmail"]);
                        break;
                    case "ME":
                        SendInvoiceEmail(model, email);
                        break;
                }
                HttpContext.Session["Invoice"] = null;
                return RedirectToAction("Invoices", "Invoice");
             }
            else
            {
                HttpContext.Session["Invoice"] = model;
                return RedirectToAction("AddnewInvoice");
            }
        }
        //Edit Invoice details
        public ActionResult EditInvoiceDetailItem(int InvoiceDetailID)
        {
            var service = new WISService();
            InvoiceDetailModel model;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();

            var Emodel = HttpContext.Session["Invoice"] as InvoiceModel;
            if (Emodel == null)
            {
                model = service.GetInvoiceDataById(InvoiceDetailID);
            }
            else
            {
                model = Emodel.InvoiceDetails[InvoiceDetailID];
            }            
           ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0 && x.ItemCategoryID == model.ItemCategoryID).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() }).ToList();

            if (model.ItemInventoryDescription == null)
            {
                model.Categories = "NonInventory";
            }
             return PartialView("_editInvoiceDetails", model);
        }
        private InvoiceResult SaveInvoice(InvoiceModel model)
        {
            var service = new WISService();
            if (string.IsNullOrEmpty(model.InvoiceJobNumber))
            {
                model.InvoiceJobNumber = "NULL";
            }
            else
            {
                model.InvoiceJobNumber = model.InvoiceJobNumber.Substring(0, 6);
            }
            //if (model.InvoiceStatus == "ME" || model.InvoiceStatus == "A")
            //{

            //}
            var res = service.SaveInvDtls(model);

            //SendInvoiceEmail(model);
            return res;
        }

        public ActionResult SaveEditInvoicedtls(InvoiceDetailModel model)
        {
            try
            {
                InvoiceModel Emodel;
                if (model.InvoiceID > 0)
                {
                    var service = new WISService();
                    Emodel = service.SaveEditInvoicesDetails(model);
                }
                else
                {
                    Emodel = HttpContext.Session["Invoice"] as InvoiceModel;
                    Emodel.InvoiceDetails[model.InvoiceDetailID] = model;
                    HttpContext.Session["Invoice"] = Emodel;
                }
                // var service = new WISService();
                // id = service.SaveEditInvoicesDetails(model);
                return View("_InvoiceDetails", Emodel.InvoiceDetails);

            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }           
        }

        public ActionResult DeleteInvoiceDetailItem(int InvoiceDetailID)
        {
            bool result = false;
            var service = new WISService();
            InvoiceModel Emodel = HttpContext.Session["Invoice"] as InvoiceModel;           
            if (Emodel.InvoiceDetails.Any())
            {
                Emodel.InvoiceDetails.RemoveAt(InvoiceDetailID);
                HttpContext.Session["Invoice"] = Emodel;
            }
            return View("_InvoiceDetails", Emodel.InvoiceDetails);
        }
        public ActionResult DeleteInvoiceDetailItemforEdit(int InvoiceId, int InvoiceDetailID)
        {
            var service = new WISService();          
            var result = service.DeleteInvoiceDetail(InvoiceDetailID, InvoiceId);
            return View("_InvoiceDetails", result.InvoiceDetails);
        }
        public void SendInvoiceEmail(InvoiceModel model, string email, string status = "Completed")
        {
            var emailService = new Shared.EmailService();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/Invoice.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{name}", GetUserFullname(model.CreatedUser));
            body = body.Replace("{0}", Request.Url.Scheme);
            body = body.Replace("{1}", Request.Url.Authority);
            body = body.Replace("{2}", model.InvoiceID.ToString());
            emailService.SendMail(email, "An Invoice " + "WIS" + model.InvoiceID + " is " + status, body, "");           
        }

        public void SendAcctInvoiceEmail(InvoiceModel model, string email, string status = "Completed")
        {
            var emailService = new Shared.EmailService();
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/AccountingInvoice.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{name}", GetUserFullname(User.Identity.Name));
            body = body.Replace("{0}", Request.Url.Scheme);
            body = body.Replace("{1}", Request.Url.Authority);
            body = body.Replace("{2}", model.InvoiceID.ToString());            

            emailService.SendMail(email, "Invoice " + "WIS" + model.InvoiceID + " is " + status, body, "");
            // return RedirectToAction("Invoices", "Invoice");
        }
        public ActionResult GetInvoiceSummary(int InvoiceId)
        {
            var service = new WISService();
            var Emodel = service.GetInvoiceDetails(InvoiceId);
            ViewBag.id = Emodel.InvoiceID;
            if (Emodel.Employee != null && Emodel.Employee != "")
            {
                var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = FirstName + " " + LastName;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();
            ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
            if (ViewBag.JCCO == 1)
            {
                ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).Job;
            }
            else
            {
                ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).Job +" "+ "CO2";
            }
            ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
            ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
            ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
            ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            //Emodel.InvoiceTotal = GetTotal(Emodel);
            HttpContext.Session["Invoice"] = null;
            return View("InvoiceSummary", Emodel);
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
        public ActionResult PrintSummaryInvoice(int invoiceId)
        {
            var service = new WISService();            
            var Emodel = service.GetInvoiceDetails(invoiceId);
            ViewBag.Creator = GetUserFullname(Emodel.CreatedUser);
            if (Emodel.Employee != null && Emodel.Employee != "")
            {
                var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = FirstName + " " + LastName;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();

            ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
            if (ViewBag.JCCO == 1)
            {
                ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).Job;
            }
            else
            {
                ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).Job + " " + "CO2";
            }
            ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
            ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
            ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
            ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            return View("PrintSummary", Emodel);
        }

        public ActionResult ExternalClientInfo()
        {
            var service = new WISService();
            var model = new ExternalClientModel();
            ViewBag.State = GetStateInfo();
            return PartialView("ExternalClient", model);
        }

        public static IEnumerable<SelectListItem> GetStateInfo()
        {
            IList<SelectListItem> states = new List<SelectListItem>
            {
                new SelectListItem() {Text="Alabama", Value="AL"},
                new SelectListItem() { Text="Alaska", Value="AK"},
                new SelectListItem() { Text="Arizona", Value="AZ"},
                new SelectListItem() { Text="Arkansas", Value="AR"},
                new SelectListItem() { Text="California", Value="CA"},
                new SelectListItem() { Text="Colorado", Value="CO"},
                new SelectListItem() { Text="Connecticut", Value="CT"},
                new SelectListItem() { Text="District of Columbia", Value="DC"},
                new SelectListItem() { Text="Delaware", Value="DE"},
                new SelectListItem() { Text="Florida", Value="FL"},
                new SelectListItem() { Text="Georgia", Value="GA"},
                new SelectListItem() { Text="Hawaii", Value="HI"},
                new SelectListItem() { Text="Idaho", Value="ID"},
                new SelectListItem() { Text="Illinois", Value="IL"},
                new SelectListItem() { Text="Indiana", Value="IN"},
                new SelectListItem() { Text="Iowa", Value="IA"},
                new SelectListItem() { Text="Kansas", Value="KS"},
                new SelectListItem() { Text="Kentucky", Value="KY"},
                new SelectListItem() { Text="Louisiana", Value="LA"},
                new SelectListItem() { Text="Maine", Value="ME"},
                new SelectListItem() { Text="Maryland", Value="MD"},
                new SelectListItem() { Text="Massachusetts", Value="MA"},
                new SelectListItem() { Text="Michigan", Value="MI"},
                new SelectListItem() { Text="Minnesota", Value="MN"},
                new SelectListItem() { Text="Mississippi", Value="MS"},
                new SelectListItem() { Text="Missouri", Value="MO"},
                new SelectListItem() { Text="Montana", Value="MT"},
                new SelectListItem() { Text="Nebraska", Value="NE"},
                new SelectListItem() { Text="Nevada", Value="NV"},
                new SelectListItem() { Text="New Hampshire", Value="NH"},
                new SelectListItem() { Text="New Jersey", Value="NJ"},
                new SelectListItem() { Text="New Mexico", Value="NM"},
                new SelectListItem() { Text="New York", Value="NY"},
                new SelectListItem() { Text="North Carolina", Value="NC"},
                new SelectListItem() { Text="North Dakota", Value="ND"},
                new SelectListItem() { Text="Ohio", Value="OH"},
                new SelectListItem() { Text="Oklahoma", Value="OK"},
                new SelectListItem() { Text="Oregon", Value="OR"},
                new SelectListItem() { Text="Pennsylvania", Value="PA"},
                new SelectListItem() { Text="Rhode Island", Value="RI"},
                new SelectListItem() { Text="South Carolina", Value="SC"},
                new SelectListItem() { Text="South Dakota", Value="SD"},
                new SelectListItem() { Text="Tennessee", Value="TN"},
                new SelectListItem() { Text="Texas", Value="TX"},
                new SelectListItem() { Text="Utah", Value="UT"},
                new SelectListItem() { Text="Vermont", Value="VT"},
                new SelectListItem() { Text="Virginia", Value="VA"},
                new SelectListItem() { Text="Washington", Value="WA"},
                new SelectListItem() { Text="West Virginia", Value="WV"},
                new SelectListItem() { Text="Wisconsin", Value="WI"},
                new SelectListItem() { Text="Wyoming", Value="WY"}
            };
            return states;
        }
        public ActionResult SaveExtClientdtls(ExternalClientModel model)
        {
            int id = 0;
            try
            {
                var service = new WISService();
                id = service.SaveExternalClientdtls(model);
            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }
            return RedirectToAction("AddnewInvoice", "Invoice");
        }

        public ActionResult GetItemInventoryList(int CategoryId)
        {
            var service = new WISService();
            var inven = HttpContext.Session["ItemInventory"] as List<ItemInventoryModel>;
            var sublst = service.GetSubItemInventory(CategoryId);
            var lst = sublst.Where(p => p.ItemInventoryQuantity > 0).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() }).ToList();
            HttpContext.Session["ItemInventory"] = sublst;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProdDetails(int Id)
        {
            var service = new WISService();
            var model = service.GetInventoryDataById(Id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateInvoiceStatus(int Id, string InvStatus)
        {
            var service = new WISService();
            var model = service.UpdateInvoiceStatus(Id, InvStatus);            
            return RedirectToAction("Invoices", "Invoice");
        }
    }
}
