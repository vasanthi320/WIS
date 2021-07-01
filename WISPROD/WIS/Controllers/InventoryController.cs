using System;
using WISServiceLayer;
using WISModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shared.Helpers;

namespace WIS.Controllers
{
    public class InventoryController : Controller
    {
        private IWISService _WISService;
        public InventoryController()
        {
                
        }
        public InventoryController(IWISService WISService)
        {
            WISService = _WISService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public static string GetUserEmail(string userName)
        {
            var info = UserUtils.FindUserInfo(userName);
            info.Email = Utils.FixMailAddress(info.Email, "hoar.com");
            return info.Email;
        }
        public ActionResult ProductsInventory()
        {
            var service = new WISService();
            string loc = service.GetEmpLocation(GetUserEmail(User.Identity.Name));// get from db
            List<ItemInventoryModel> Iim = new List<ItemInventoryModel>();
            var model = service.GetInventoryData();  
            
           var lstLoc =service.GetLocationList().
                Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationDescription.ToString() ,Selected = p.LocationDescription == loc ?true : false }).ToList();
            lstLoc.Add(new SelectListItem { Text = "All", Value = "All", Selected = "All" == loc ? true : false });
            //var lstid= service.GetLocationList().
            //    Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString(), Selected = p.LocationDescription == loc ? true : false }).ToList();
            //lstLoc.Add(new SelectListItem { Text = "All", Value = "All", Selected = "All" == loc ? true : false });
            ViewBag.Location = lstLoc;
           
            return View(model);
        }
        //Add prod line item
        public ActionResult AddnewProdITemTypeLineItem()
        {
            var service = new WISService();
            var model = new ItemInventoryModel();
            model.CreatedUser = User.Identity.Name;
            model.ModUser = User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            return PartialView("_AddProductInventory", model);
        }
        //AddProdITemtoexist
        public ActionResult AddProdITemtoexist(string Location)
        {
            var service = new WISService();
            var model = new ItemInventoryModel();
            model.CreatedUser = User.Identity.Name;
            model.ModUser = User.Identity.Name;
            model.CreatedDate = DateTime.Now;
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();          
            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            ViewBag.Quantity = model.ItemInventoryQuantity;
            return PartialView("_AddExisProdInventory", model);
        }
        //Edit for existing item
        public ActionResult EditPrdExisInventory(int id)
        {
            var service = new WISService();
            List<ItemInventoryModel> Iim = new List<ItemInventoryModel>();
            //ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString()}).ToList();

            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            var model = service.GetInventoryDataById(id);
            ViewBag.Inventory = service.GetSubItemInventory(model.ItemCategoryID.Value).Where(x => x.ItemInventoryQuantity != 0).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() }).ToList();
            ViewBag.Quantity =model.ItemInventoryQuantity+" +";
            model.ModUser = User.Identity.Name;
            if (model.ItemInventoryID > 0)
            {
                return PartialView("_AddExisProdInventory", model);
            }
            return PartialView("_AddProductInventory", model);
        }     

        public ActionResult GetProdDetails(int Id)
        {
            var service = new WISService();
            var model = service.GetInventoryDataById(Id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //Edit Line Item
        public ActionResult EditPrdInventory(int id)
        {
            var service = new WISService();
            List<ItemInventoryModel> Iim = new List<ItemInventoryModel>();
            ViewBag.Inventory = service.GetInventoryData().Where(x => x.ItemInventoryQuantity != 0).Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() + "|" + p.ItemInventorySalesPrice + "|" + p.ItemInventoryQuantity }).ToList();
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            ViewBag.Location = service.GetLocationList().Select(p => new SelectListItem { Text = p.LocationDescription, Value = p.LocationID.ToString() }).ToList();
            var model = service.GetInventoryDataById(id);
            model.ModUser = User.Identity.Name;
            ViewBag.Active = service.GetInventoryDataById(id);
            if (model.ItemInventoryID > 0)
            {
                return PartialView("_editProdInvDocs", model);
            }
            return PartialView("_AddProductInventory", model);
        }
        public ActionResult EditPrdDescInventory(string Location)
        {
            var service = new WISService();
            var model = new ItemCategory();           
            ViewBag.Category = service.GetItemCategories().Select(p => new SelectListItem { Text = p.ItemCategoryDescription, Value = p.ItemCategoryID.ToString() }).ToList();
            
            return PartialView("_EditPrdDescInventory", model);           
        }
        //Save Items for edits
        public ActionResult SaveAddEditItemProdTypes(ItemInventoryModel model)
        {
            int id = 0;          
            try
            {
                var service = new WISService();
                model.CreatedUser = User.Identity.Name;
                var emodel = service.GetInventoryDataById(model.ItemInventoryID);
                model.Active = emodel.Active;        
                if(emodel.ItemInventorySalesPrice!=model.ItemInventorySalesPrice)
                {
                    model.ItemInventorySalesPrice = service.SaveEditItmSalesPrice(model);
                }
                id = service.SaveEditItmProd(model);                
            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }
            return RedirectToAction("ProductsInventory", "Inventory");
        }
        public ActionResult SaveAddEditItemProdTypes1(ItemInventoryModel model)
        {
            int id = 0;

            try
            {
                var service = new WISService();
                model.CreatedUser = User.Identity.Name;
                var emodel = service.GetInventoryDataById(model.ItemInventoryID);
                model.ItemInventoryNumber = emodel.ItemInventoryNumber;
                model.ItemInventoryReplacementCost = emodel.ItemInventoryReplacementCost;
                model.ItemInventorySalesPrice = emodel.ItemInventorySalesPrice;
                model.ItemInventoryDescription = emodel.ItemInventoryDescription;
                id = service.SaveEditItmProd(model);
            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }
            return RedirectToAction("ProductsInventory", "Inventory");
        }
        //Save Items for new Inventory
        public ActionResult SaveAddNewItemProdTypes(ItemInventoryModel model)
        {
            int id = 0;

            try
            {
                var service = new WISService();
                model.CreatedUser = User.Identity.Name;
                model.Active = true;
                id = service.SaveEditItmProd(model);
            }
            catch (Exception ex)
            {
                throw new Exception("could not  Save The Details", ex);
            }
            return RedirectToAction("ProductsInventory", "Inventory");
        }
        public ActionResult SaveEditItemProdDesc(ItemCategory model)
        {
            var service = new WISService();
           var emodel = service.SaveItmProdDesc(model);
            return RedirectToAction("ProductsInventory", "Inventory"); ;
        }
        //Get ItemInventory List
        public ActionResult GetItemInventoryList(int CategoryId)
        {           
            var service = new WISService();
            var inven = HttpContext.Session["ItemInventory"] as List<ItemInventoryModel>;
            var sublst = service.GetSubItemInventory(CategoryId);
            var lst = sublst.Select(p => new SelectListItem { Text = p.ItemInventoryDescription, Value = p.ItemInventoryID.ToString() }).ToList();            
            HttpContext.Session["ItemInventory"] = sublst;
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLowStockList(int location)
        {
            var service = new WISService();           
            var model = service.GetLowStockInventory(location);
            ViewBag.StockDeatils = "Low Stock Details";
            return View("GetStockList", model);
        }
        public ActionResult GetOutOfStockList(int location)
        {
            var service = new WISService();
            var model = service.GetOutOfStockInventory(location);
            ViewBag.StockDeatils = "Out Of Stock Details";
            return View("GetStockList", model);
        }
        public ActionResult GetAllLowStockList(int location)
        {
            var service = new WISService();
            var model = service.GetLowStockInventory(1);
            var emodel=service.GetLowStockInventory(2);
            ViewBag.StockDeatils = "All Low Stock Details";
            return View("GetStockList", model.Concat(emodel));
        }
        public ActionResult GetAllOutStockList(int location)
        {
            var service = new WISService();
            var model = service.GetLowStockInventory(1);
            var emodel = service.GetLowStockInventory(2);
            ViewBag.StockDeatils = "All Out Of Stock Details";
            return View("GetStockList", model.Concat(emodel));
        }
        public ActionResult Reports()
        {
            return View();
        }
       public ActionResult SaveActiveStatus(bool Ac,int ID)
        {
            var service = new WISService();
            var st = service.UpdateInventoryActStatus(Ac,ID);
            return View();
        }
    }
}