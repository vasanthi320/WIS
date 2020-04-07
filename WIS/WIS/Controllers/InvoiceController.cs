using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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
        public ActionResult Estimates()
        {
            var service = new WISService();
            List<EstimateModel> est = new List<EstimateModel>();
            var model = service.GetEstimatesList();
            return View(model);
        }
        public JsonResult GetProjects(string term)
        {
            var service = new WISService();
            //List<InvoiceModel> Iim = new List<InvoiceModel>();
            var Joblist = service.GetJobDetails();
            var jobName = Joblist.Where(n => (n.JobDescription).StartsWith(term));
            return Json(jobName, JsonRequestBehavior.AllowGet);
        }      
        public JsonResult GetCategories(string term)
        {
            var service = new WISService();
            //List<InvoiceDetailModel> Iim = new List<InvoiceDetailModel>();
            var Catlist = service.GetItemCategories();
            var CategoryName = Catlist.Where(n => (n.ItemCategoryDescription).StartsWith(term))/*.OrderByDescending(p => p.ItemCategoryDescription).ToList()*/;
            return Json(CategoryName, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult EditInvoice(int InvoiceId)
        {
            var service = new WISService();
            HttpContext.Session["Invoice"] = null;           
            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p => p.Email == useremail).SingleOrDefault().EmployeeID;
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp != null ? emp.LocationID : 1;
           
            var model = service.GetInvoiceDetails(InvoiceId);
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            //ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job.ToString() }).ToList();
            //Changed on 02/24/2020
            ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text =p.JobDescription, Value = p.Job.ToString() }).ToList();
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString(),Selected=p.EmployeeID.ToString()==model.Employee}).ToList();
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().
               Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString()}).ToList();
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            if (model.Employee != null && model.Employee != "" && model.Employee != "NULL")
            {               
                //ViewBag.Employee1 = service.GetEmployeeDtlsById(Convert.ToInt32(model.Employee)).FirstName;
                ViewBag.Employee1 = model.Employee;
            }
            model.defaultdetails = service.GetDefaultDetailsfromDb();

            if (model.InvoiceID > 0)
            {               
                return View("editInvoice", model);
            }
            
            model.LocationID = Emplocation;
            return View("AddInvoiceItem", model);
        }
        public ActionResult EditEstimate(int Estimate)
        {
            var service = new WISService();
            HttpContext.Session["Estimates"] = null;
            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p => p.Email == useremail).SingleOrDefault().EmployeeID;
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp != null ? emp.LocationID : 1;

            var model = service.GetEstimateDetails(Estimate);
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();           
            //Changed on 03/09/2020
            ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.JobDescription, Value = p.Job.ToString() }).ToList();
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString(), Selected = p.EmployeeID.ToString() == model.Employee }).ToList();
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().
               Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            if (model.Employee != null && model.Employee != "" && model.Employee != "NULL")
                ViewBag.Employee1 = service.GetEmployeeDtlsById(Convert.ToInt32(model.Employee)).FirstName;
            model.defaultdetails = service.GetDefaultDetailsfromDb();

            if (model.Estimate > 0)
            {                
                return View("editEstimate", model);
            }

            model.LocationID = Emplocation;
            return View("AddEstimateItem", model);
        }
        public ActionResult AddnewInvoice(string mesg = "")
        {
            var service = new WISService();
            ViewBag.Result = mesg;
                         
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString() }).ToList();

            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p=>p.Email==useremail).SingleOrDefault().EmployeeID;
          
            //var defaultDetails = service.GetDefaultValue(id);
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp !=null? emp.LocationID : 1;
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();

            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
           
            
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            var model = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();
            model.InvoiceDetails = model.InvoiceDetails ?? new List<InvoiceDetailModel>();            
            model.ClientTypeID = 2;
            model.LocationID = Emplocation;
            model.defaultdetails = service.GetDefaultDetailsfromDb();
            return View("AddInvoiceItem", model);            
        }
        public ActionResult AddnewEstimate(string mesg = "")
        {
            var service = new WISService();
            ViewBag.Result = mesg;                              
            ViewBag.Employee = service.GetEmployeeDetails().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.EmployeeID.ToString() }).ToList();

            var useremail = GetUserEmail(User.Identity.Name);
            var empdetails = service.GetEmployeeDetails().Where(p => p.Email == useremail).SingleOrDefault().EmployeeID;            
            var emp = service.GetEmpLocation(empdetails);
            var Emplocation = emp != null ? emp.LocationID : 1;
            ViewBag.client = service.GetClient().Select(p => new SelectListItem { Text = p.ClientTypeDescription, Value = p.ClientTypeID.ToString() }).ToList();

            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            ViewBag.ExternalClient = service.GetExternalClientList().Select(p => new SelectListItem { Text = p.FirstName + " " + p.LastName, Value = p.ExternalClientID.ToString() }).ToList();
            var model = HttpContext.Session["Estimates"] as EstimateModel ?? new EstimateModel();
            model.EstimateDetails = model.EstimateDetails ?? new List<EstimateDetailModel>();
            model.ClientTypeID = 2;
            model.LocationID = Emplocation;
            model.defaultdetails = service.GetDefaultDetailsfromDb();
            return View("AddEstimateItem", model);
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
            var Inventorylst = new List<SelectListItem>();            
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });                    
            ViewBag.Inventory = Inventorylst;  
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
           
            return View("_popupInvoiceItem", model);
        }
        public ActionResult PopupAddEstimateItem(int Location)
        {
            var service = new WISService();
            var model = new EstimateDetailModel();          
            var Inventorylst = new List<SelectListItem>();
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });
            ViewBag.Inventory = Inventorylst;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            return View("_popupEstimateItem", model);
        }
        public ActionResult EditPopupAddInvoiceItem(int Location)
        {
            var service = new WISService();
            var model = new InvoiceDetailModel();
            var Inventorylst = new List<SelectListItem>();
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });
            ViewBag.Inventory = Inventorylst;            
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();          
            return View("_editpopupInvoiceItem", model);
        }
        public ActionResult EditPopupAddEstimateItem(int Location)
        {
            var service = new WISService();
            var model = new EstimateDetailModel();
            var Inventorylst = new List<SelectListItem>();
            Inventorylst.Add(new SelectListItem { Text = "Yes", Value = "1" });
            Inventorylst.Add(new SelectListItem { Text = "No", Value = "2" });
            ViewBag.Inventory = Inventorylst;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            return View("_editpopupEstimateItem", model);
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
        [HttpPost]
        public ActionResult AddEstimateItems(int Id, int qty, decimal price, string CGCode, string desc, int cat, string catText, int locaton)
        {
            var lst = HttpContext.Session["Estimates"] as EstimateModel ?? new EstimateModel();
            if (lst.EstimateDetails == null)
            {
                lst.EstimateDetails = new List<EstimateDetailModel>();
            }
            lst.EstimateDetails.Add(new EstimateDetailModel { EstimateDetailID = lst.EstimateDetails.Count, EstimateDescription = desc, ItemInventoryID = Id, EstimateDetailQuantity = qty, ItemInventoryCost = price, EstimateDetailLineItemTotal = qty * price, EstimateDetailCostCodeGL = CGCode, Categories = catText, ItemCategoryID = cat });
            lst.Total = GetEstimateTotal(lst);
            ViewBag.Total = lst.Total;
            HttpContext.Session["Estimates"] = lst;
            lst.LocationID = locaton;
            return View("_estimateDetails", lst.EstimateDetails);
        }
        [HttpPost]
        public ActionResult saveTermsValtoDb(string termsVal)
        {
            var service = new WISService();           
           var model= service.UpdateDefaultValue(termsVal);
            return View("AddInvoiceItem", model);
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
        public ActionResult AddEstimateItems1(int Id, int qty, decimal price, string CGCode, string itm1, int EstimateId)
        {
            var lst = HttpContext.Session["Estimates"] as EstimateModel ?? new EstimateModel();
            if (lst.EstimateDetails == null)
            {
                lst.EstimateDetails = new List<EstimateDetailModel>();
            }
            lst.EstimateDetails.Add(new EstimateDetailModel { EstimateDetailID = lst.EstimateDetails.Count, EstimateDetailQuantity = qty, ItemInventoryCost = price, EstimateDetailLineItemTotal = qty * price, EstimateDetailCostCodeGL = CGCode, NonInventoryItem = itm1 });
            lst.Total = GetEstimateTotal(lst);
            ViewBag.Total = lst.Total;
            HttpContext.Session["Estimates"] = lst;
            return View("_estimateDetails", lst.EstimateDetails);
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
        public ActionResult EditAddEstimateItems(int Id, int qty, decimal price, string CGCode, string desc, int cat, string catText, int EstimateId)
        {
            var service = new WISService();
            bool sucess = service.SaveEstimateItem(EstimateId, new EstimateDetailModel { EstimateDescription = desc, ItemInventoryID = Id, EstimateDetailQuantity = qty, ItemInventoryCost = price, EstimateDetailLineItemTotal = qty * price, EstimateDetailCostCodeGL = CGCode, Categories = catText, ItemCategoryID = cat });
            var lst = service.GetEstimateDetails(EstimateId);
            lst.Total = GetEstimateTotal(lst);
            ViewBag.Total = lst.Total;
            return View("_estimateDetails", lst.EstimateDetails);
        }
        public ActionResult EditAddEstimateItems1(int Id, int qty, decimal price, string CGCode, string itm1, int EstimateId)
        {
            var service = new WISService();
            bool sucess = service.SaveEstimateItem(EstimateId, new EstimateDetailModel { ItemInventoryID = Id == 0 ? (int?)null : Id, EstimateDetailQuantity = qty, ItemInventoryCost = price, EstimateDetailLineItemTotal = qty * price, EstimateDetailCostCodeGL = CGCode, NonInventoryItem = itm1 });
            var lst = service.GetEstimateDetails(EstimateId);
            lst.Total = GetEstimateTotal(lst);
            ViewBag.Total = lst.Total;
            return View("_estimateDetails", lst.EstimateDetails);
        }
        public ActionResult GetInvoiceItems()
        {
            var lst = HttpContext.Session["Invoice"] as InvoiceModel ?? new InvoiceModel();
            return View("_InvoiceDetails", lst.InvoiceDetails);
        }
        public ActionResult GetEstimateItems()
        {
            var lst = HttpContext.Session["Estimates"] as EstimateModel ?? new EstimateModel();
            return View("_estimateDetails", lst.EstimateDetails);
        }
        private decimal GetTotal(InvoiceModel e)
        {
            decimal m = 0;
            if (e.InvoiceDetails != null && e.InvoiceDetails.Any()) { m = e.InvoiceDetails.Sum(p => p.InvoiceDetailLineItemTotal.Value); }
            var amt = Math.Round(m, 2);
            return amt;
        }
        private decimal GetEstimateTotal(EstimateModel e)
        {
            decimal m = 0;
            if (e.EstimateDetails != null && e.EstimateDetails.Any()) { m = e.EstimateDetails.Sum(p => p.EstimateDetailLineItemTotal.Value); }
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
        //Save Estimates
        public ActionResult SaveEstimateDetails(EstimateModel model)
        {           
            var Emodel = HttpContext.Session["Estimates"] as EstimateModel;
            var service = new WISService();
            model.EstimateDetails = Emodel.EstimateDetails;
            Emodel.Total = GetEstimateTotal(Emodel);
            model.Total = Emodel.Total;
            model.CreatedUser = User.Identity.Name;
            var email = GetUserEmail(model.CreatedUser);
            var res = SaveEstimate(model);
            if (res.Status)
            {
                model.Estimate = res.InvoiceId;
                switch (model.Status)
                {

                    case "D": break;                  
                    case "ME":
                        SendEstimateEmail(model, email);
                        break;
                }
                HttpContext.Session["Estimates"] = null;
                return RedirectToAction("Estimates", "Invoice");
            }
            else
            {
                HttpContext.Session["Estimates"] = model;
                return RedirectToAction("AddnewEstimate", new { mesg = res.Inventory });
            }
        }
        //Save Edit Invoice
        public ActionResult SaveEditInvoiceDetails(InvoiceModel model)
        {           
            var service = new WISService();           
            model.CreatedUser = User.Identity.Name;
            var email = GetUserEmail(model.CreatedUser);           
            var res =!string.IsNullOrEmpty(model.InvoiceStatus);            
            var emodel = service.updateInvoiceDetail(model);           
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
                HttpContext.Session["Invoice"] = emodel;
                return RedirectToAction("AddnewInvoice");
            }
        }
        //Save Edit Estimate 
        public ActionResult SaveEditEstimateDetails(EstimateModel model)
        {
            var service = new WISService();
            model.CreatedUser = User.Identity.Name;
            var email = GetUserEmail(model.CreatedUser);           
            var res = !string.IsNullOrEmpty(model.Status);
             var emodel = service.updateEstimateDetail(model);
            
            if (res)
            {                
                switch (model.Status)
                {
                    case "D": break;                       
                    case "ME":
                        SendEstimateEmail(model, email);
                        break;
                }
                HttpContext.Session["Estimates"] = null;
                return RedirectToAction("Estimates", "Invoice");
            }
            else
            {
                HttpContext.Session["Estimates"] = emodel;
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
        //Edit Estimate details
        public ActionResult EditEstimateDetailItem(int EstimateDetailID)
        {
            var service = new WISService();
            EstimateDetailModel model;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();

            var Emodel = HttpContext.Session["Estimates"] as EstimateModel;
            if (Emodel == null)
            {
                model = service.GetEstimateDataById(EstimateDetailID);
            }
            else
            {
                model = Emodel.EstimateDetails[EstimateDetailID];
            }
            ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0 && x.ItemCategoryID == model.ItemCategoryID).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() }).ToList();

            if (model.EstimateDescription == null)
            {
                model.Categories = "NonInventory";
            }
            return PartialView("_editEstimateDetails", model);
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
            var res = service.SaveInvDtls(model);

            //SendInvoiceEmail(model);
            return res;
        }
        private InvoiceResult SaveEstimate(EstimateModel model)
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
            var res = service.SaveEstimateDtls(model);

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
                return View("_InvoiceDetails", Emodel.InvoiceDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }           
        }
        public ActionResult SaveEditEstimatedtls(EstimateDetailModel model)
        {
            try
            {
                EstimateModel Emodel;
                if (model.EstimateID > 0)
                {
                    var service = new WISService();
                    //Emodel = service.SaveEditInvoicesDetails(model);
                    Emodel = service.SaveEditEstimateDetails(model);
                }
                else
                {
                    Emodel = HttpContext.Session["Estimates"] as EstimateModel;
                    Emodel.EstimateDetails[model.EstimateDetailID] = model;
                    HttpContext.Session["Estimates"] = Emodel;
                }
                return View("_estimateDetails", Emodel.EstimateDetails);
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
        public ActionResult DeleteEstimateDetailItem(int EstimateDetailId)
        {
            bool result = false;
            var service = new WISService();
            EstimateModel Emodel = HttpContext.Session["Estimates"] as EstimateModel;
            if (Emodel.EstimateDetails.Any())
            {
                Emodel.EstimateDetails.RemoveAt(EstimateDetailId);
                HttpContext.Session["Estimates"] = Emodel;
            }
            return View("_estimateDetails", Emodel.EstimateDetails);
        }
        public ActionResult DeleteInvoiceDetailItemforEdit(int InvoiceId, int InvoiceDetailID)
        {
            var service = new WISService();          
            var result = service.DeleteInvoiceDetail(InvoiceDetailID, InvoiceId);
            return View("_InvoiceDetails", result.InvoiceDetails);
        }
        public ActionResult DeleteEstimateDetailItemforEdit(int estimate, int EstimateDetailID)
        {
            var service = new WISService();
            var result = service.DeleteEstimateDetail(EstimateDetailID, estimate);
            return View("_InvoiceDetails", result.EstimateDetails);
        }
        //Changed on 2/26/2020
        //public void SendInvoiceEmail(InvoiceModel model, string email, string status = "Completed")
        public void SendInvoiceEmail(InvoiceModel model, string email, string status = "Drafted")
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
        public void SendEstimateEmail(EstimateModel model, string email, string status = "Drafted")
        {
            var emailService = new Shared.EmailService();

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Content/Email/Estimate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{name}", GetUserFullname(model.CreatedUser));
            body = body.Replace("{0}", Request.Url.Scheme);
            body = body.Replace("{1}", Request.Url.Authority);
            body = body.Replace("{2}", model.Estimate.ToString());
            emailService.SendMail(email, "An Estimate " + "WES" + model.Estimate + " is " + status, body, "");
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
        }
        public ActionResult GetInvoiceSummary(int InvoiceId)
        {
            var service = new WISService();
            var Emodel = service.GetInvoiceDetails(InvoiceId);
            ViewBag.id = Emodel.InvoiceID;
            if (Emodel.Employee != null && Emodel.Employee != "")
            {
                //var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                //var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = Emodel.Employee;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            if (Emodel.InvoiceJobNumber != null && Emodel.InvoiceJobNumber != "" && Emodel.InvoiceJobNumber != "NULL")
            {
                ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();
                ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
                if (ViewBag.JCCO == 1)
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription;
                }
                else
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription + " " + "CO2";
                }

                ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
                ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
                ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
                ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            }
            //Emodel.InvoiceTotal = GetTotal(Emodel);
            HttpContext.Session["Invoice"] = null;
            return View("InvoiceSummary", Emodel);
        }
        public ActionResult GetEstimateSummary(int Estimate)
        {
            var service = new WISService();
            var Emodel = service.GetEstimateDetails(Estimate);
            ViewBag.id = Emodel.Estimate;
            if (Emodel.Employee != null && Emodel.Employee != "")
            {
                var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = FirstName + " " + LastName;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            if (Emodel.InvoiceJobNumber != null && Emodel.InvoiceJobNumber != "" && Emodel.InvoiceJobNumber != "NULL")
            {
                ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();
                ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
                if (ViewBag.JCCO == 1)
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription;
                }
                else
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription + " " + "CO2";
                }

                ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
                ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
                ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
                ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            }           
            HttpContext.Session["Estimates"] = null;
            return View("EstimateSummary", Emodel);
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
                //var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                //var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = Emodel.Employee;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            if (Emodel.InvoiceJobNumber != null && Emodel.InvoiceJobNumber != "" && Emodel.InvoiceJobNumber != "NULL")
            {
                ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();
                ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
                if (ViewBag.JCCO == 1)
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription;
                }
                else
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription + " " + "CO2";
                }
                ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
                ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
                ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
                ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            }
            return View("PrintSummary", Emodel);
        }
        public ActionResult PrintSummaryEstimate(int estimate)
        {
            var service = new WISService();
            var Emodel = service.GetEstimateDetails(estimate);
            ViewBag.Creator = GetUserFullname(Emodel.CreatedUser);
            if (Emodel.Employee != null && Emodel.Employee != "")
            {
                var FirstName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).FirstName;
                var LastName = service.GetEmployeeDtlsById(Convert.ToInt32(Emodel.Employee)).LastName;
                ViewBag.EmployeeName = FirstName + " " + LastName;
                Emodel.Employee = ViewBag.EmployeeName;
            }
            if (Emodel.InvoiceJobNumber != null && Emodel.InvoiceJobNumber != "" && Emodel.InvoiceJobNumber != "NULL")
            {
                ViewBag.Job = service.GetJobDetails().Select(p => new SelectListItem { Text = p.Job + "" + p.JobDescription, Value = p.Job + "" + p.JobDescription.ToString() }).ToList();
                ViewBag.JCCO = service.GetJobDetails(Emodel.InvoiceJobNumber).JCCo;
                if (ViewBag.JCCO == 1)
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription;
                }
                else
                {
                    ViewBag.JobDes = service.GetJobDetails(Emodel.InvoiceJobNumber).JobDescription + " " + "CO2";
                }
                ViewBag.MailAddress = service.GetJobDetails(Emodel.InvoiceJobNumber).MailAddress;
                ViewBag.MailCity = service.GetJobDetails(Emodel.InvoiceJobNumber).MailCity;
                ViewBag.MailState = service.GetJobDetails(Emodel.InvoiceJobNumber).MailState;
                ViewBag.MailZip = service.GetJobDetails(Emodel.InvoiceJobNumber).MailZip;
            }
            return View("PrintEstSummary", Emodel);
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
            if (model.InvoiceStatus == "Completed")
            {
               UpdateInvONCompletedStatus(model);
            }
            return RedirectToAction("Invoices", "Invoice");
        }
        private InvoiceResult UpdateInvONCompletedStatus(InvoiceModel model)
        {
            var service = new WISService();           
            var res = service.SaveInvDtls(model);
            return res;
        }
        public ActionResult ConvertEsttoInv(int EstimateID)
        {
            var service = new WISService();
            var res = service.UpdateEstimateStatus(EstimateID);           
            var estde = service.ConvEsttoInv(EstimateID);   
            return RedirectToAction("Invoices", "Invoice");
        }
        public void ArchiveMonthInvoices(int[] ArchInvoices)
        {
            var service = new WISService();
            var model = new InvoiceModel();
            foreach (var item in ArchInvoices)
            {
                DateTime dateTime = DateTime.UtcNow.Date;               
                var InvLst = service.GetInvoiceDetails(item);                
                if (InvLst.InvoiceDate.Month < dateTime.Month && InvLst.InvoiceDate.Month!=dateTime.Month) { 
                    var res = service.BulkUpdateInvoiceStatus(item);
                }
                else
                {

                }
            }           
        }
        public ActionResult ArchivedInvoices()
        {
            var service = new WISService();
            List<InvoiceModel> Iim = new List<InvoiceModel>();
            var model = service.GetArchivedInvoiceList();
            return View(model);
        }
       
    }
}
