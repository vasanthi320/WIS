using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WISData;
using WISModels;

namespace WISServiceLayer
{
    public class WISService : IWISService
    {
        //private const double V = 0.3000;
        private readonly WISEntities _context;
        public WISService()
        {
            _context = new WISEntities();
        }
        public IList<ItemInventoryModel> GetInventoryData()
        {
            var Invlst = (from ti in _context.tblItemInventories
                          join loc in _context.tblLocations
                          on ti.LocationID
                          equals loc.LocationID
                          select new ItemInventoryModel
                          {
                              ItemInventoryID = ti.ItemInventoryID,
                              ItemInventoryNumber = ti.ItemInventoryNumber,
                              ItemInventoryDescription = ti.ItemInventoryDescription,
                              ItemCategoryID = ti.ItemCategoryID,
                              ItemInventoryQuantity = ti.ItemInventoryQuantity,
                              LocationID = ti.LocationID,
                              LocationDescription = loc.LocationDescription,
                              ItemInventoryReorderPoint = ti.ItemInventoryReorderPoint,
                              ItemInventoryReplacementCost = Math.Round(ti.ItemInventoryReplacementCost ?? 0, 2),
                              ItemInventoryMarkup = ti.ItemInventoryMarkup,
                              ItemInventorySalesPrice = Math.Round(ti.ItemInventorySalesPrice ?? 0, 2),
                              ItemInventoryReplacementCostOnHand = ti.ItemInventoryReplacementCostOnHand,
                              ItemInventoryExtendedCost = Math.Round(ti.ItemInventoryExtendedCost ?? 0, 2),
                              CreatedDate = ti.CreatedDate,
                              CreatedUser = ti.CreatedUser,
                              ModDate = ti.ModDate,
                              ModUser = ti.ModUser
                          }).ToList();
            var slsprtotal = Invlst.Sum(p => p.ItemInventorySalesPrice);
            var replcsttotal = Invlst.Sum(p => p.ItemInventoryReplacementCost);
            return Invlst;
        }

        public InvoiceSummary getInSummary(InvoiceSummary model)
        {
            var lst = _context.tblInvoiceSummaries.Select(p => new InvoiceSummary
            {
                InvoiceSummaryTotal0To30 = p.InvoiceSummaryTotal0To30,
                InvoiceSummaryTotal31To60 = p.InvoiceSummaryTotal31To60,
                InvoiceSummaryTotal61To90 = p.InvoiceSummaryTotal61To90,
                InvoiceSummaryTotal91Plus = p.InvoiceSummaryTotal91Plus,
                InvoiceSummaryTotalAll = p.InvoiceSummaryTotalAll,
                UpdatedDate = p.UpdatedDate
            }).SingleOrDefault();
            return lst;
        }
        public List<DefaultModel> GetDefaultDetailsfromDb()
        {
            var emp = _context.tblDefaults.Select(p => new DefaultModel
            {
                DefaultID = p.DefaultID,
                DefaultName = p.DefaultName,
                DefaultValue = p.DefaultValue
            });
            return emp.ToList();
        }
        public DefaultModel UpdateDefaultValue(string termsVal)
        {
            tblDefault emodel;
            using (var c = new WISEntities())
            {
                emodel = c.tblDefaults.SingleOrDefault(p => p.DefaultID == 1);
                emodel.DefaultValue = termsVal;
                c.SaveChanges();
            }
            return new DefaultModel { DefaultID = 1, DefaultName = emodel.DefaultName, DefaultValue = emodel.DefaultValue };
        }
        public List<ItemCategory> GetItemCategories()
        {
            var lst = _context.tblItemCategories.Select(p => new ItemCategory
            {
                ItemCategoryID = p.ItemCategoryID,
                ItemCategoryDescription = p.ItemCategoryDescription,

            }).ToList();
            return lst;
        }
        public int GetCategoryById(int Id)
        {
            var lst = (from id in _context.tblInvoiceDetails
                       join In in _context.tblItemInventories on id.ItemInventoryID equals In.ItemInventoryID
                       where id.InvoiceDetailID == Id
                       select new InvoiceDetailModel
                       {
                           InvoiceDetailID = id.InvoiceDetailID,
                           InvoiceID = id.InvoiceID,
                           ItemInventoryID = id.ItemInventoryID,
                           NonInventoryItem = id.NonInventoryItem,
                           InvoiceDetailCostCodeGL = id.InvoiceDetailCostCodeGL,
                           ItemInventoryUnitCost = id.ItemInventoryUnitCost,
                           InvoiceDetailQuantity = id.InvoiceDetailQuantity,
                           InvoiceDetailLineItemTotal = id.InvoiceDetailLineItemTotal,
                           ItemCategoryID = In.ItemCategoryID
                       });
            var res = lst.SingleOrDefault();
            if (res != null)
                return res.ItemCategoryID.Value;
            else
                return 0;

        }
        public int GetCategoryByEstId(int estId)
        {
            var lst = (from id in _context.tblEstimateDetails
                       join In in _context.tblItemInventories on id.ItemInventoryID equals In.ItemInventoryID
                       where id.EstimateDetailID == estId
                       select new EstimateDetailModel
                       {
                           EstimateDetailID = id.EstimateDetailID,
                           EstimateID = id.EstimateID,
                           ItemInventoryID = id.ItemInventoryID,
                           NonInventoryItem = id.NonInventoryItem,
                           EstimateDetailCostCodeGL = id.EstimateDetailCostCodeGL,
                           ItemInventoryCost = id.ItemInventoryCost,
                           EstimateDetailQuantity = id.EstimateDetailQuantity,
                           EstimateDetailLineItemTotal = id.EstimateDetailLineItemTotal,
                           ItemCategoryID = In.ItemCategoryID
                       });
            var res = lst.SingleOrDefault();
            if (res != null)
                return res.ItemCategoryID.Value;
            else
                return 0;

        }
        public IList<LocationModel> GetLocationList()
        {

            var lst = _context.tblLocations.Select(p => new LocationModel
            {
                LocationID = p.LocationID,
                LocationDescription = p.LocationDescription,

            }).ToList();
            return lst;
        }
        public EmployeeLocation GetEmpLocation(int employeId)
        {
            return _context.tblEmployeeLocations.Where(p => p.EmployeeID == employeId).Select(p => new EmployeeLocation
            {
                EmployeeID = p.EmployeeID,
                LocationID = p.LocationID,
                UseAllAsDefault = p.UseAllAsDefault
            }).FirstOrDefault();


        }
        public ItemInventoryModel GetInventoryDataById(int id)
        {
            var itm = _context.tblItemInventories.Find(id);
            var it = new ItemInventoryModel
            {
                ItemInventoryID = itm.ItemInventoryID,
                ItemInventoryNumber = itm.ItemInventoryNumber,
                ItemInventoryDescription = itm.ItemInventoryDescription,
                ItemCategoryID = itm.ItemCategoryID,
                ItemInventoryQuantity = itm.ItemInventoryQuantity,
                LocationID = itm.LocationID,
                ItemInventoryReorderPoint = itm.ItemInventoryReorderPoint,
                ItemInventoryReplacementCost = itm.ItemInventoryReplacementCost,
                ItemInventoryMarkup = itm.ItemInventoryMarkup,
                ItemInventorySalesPrice = itm.ItemInventorySalesPrice,
                ItemInventoryReplacementCostOnHand = itm.ItemInventoryReplacementCostOnHand,
                ItemInventoryExtendedCost = itm.ItemInventoryExtendedCost,
                CreatedDate = itm.CreatedDate,
                CreatedUser = itm.CreatedUser,
                ModDate = itm.ModDate,
                ModUser = itm.ModUser
            };
            return it;
        }

        public InvoiceModel DeleteInvoiceDetail(int invoiceDetailID, int invoiceId)
        {
            var invoiceitm = _context.tblInvoices.Find(invoiceId);
            var itm = invoiceitm.tblInvoiceDetails.SingleOrDefault(p => p.InvoiceDetailID == invoiceDetailID);
            _context.tblInvoiceDetails.Remove(itm);
            _context.SaveChanges();
            invoiceitm.InvoiceTotal = invoiceitm.tblInvoiceDetails.Sum(j => j.InvoiceDetailLineItemTotal);
            var s = _context.SaveChanges() > 0;
            var im = MapInvoice(invoiceitm);
            return im;
        }
        public EstimateModel DeleteEstimateDetail(int EstimateDetailID, int estimate)
        {
            var invoiceitm = _context.tblEstimates.Find(estimate);
            var itm = invoiceitm.tblEstimateDetails.SingleOrDefault(p => p.EstimateDetailID == EstimateDetailID);
            _context.tblEstimateDetails.Remove(itm);
            _context.SaveChanges();
            invoiceitm.Total = invoiceitm.tblEstimateDetails.Sum(j => j.EstimateDetailLineItemTotal);
            var s = _context.SaveChanges() > 0;
            var im = MapEstimate(invoiceitm);
            return im;
        }
        public InvoiceDetailModel GetInvoiceDataById(int id)
        {
            var itm = _context.tblInvoiceDetails.Find(id);
            var it = new InvoiceDetailModel
            {
                ItemInventoryID = itm.ItemInventoryID,
                InvoiceDetailID = itm.InvoiceDetailID,
                ItemCategoryID = GetCategoryById(itm.InvoiceDetailID),
                InvoiceDetailCostCodeGL = itm.InvoiceDetailCostCodeGL,
                NonInventoryItem = itm.NonInventoryItem,
                InvoiceDetailLineItemTotal = itm.InvoiceDetailLineItemTotal,
                InvoiceID = itm.InvoiceID,
                InvoiceDetailQuantity = itm.InvoiceDetailQuantity,
                ItemInventoryDescription = itm.ItemInventoryDescription,
                ItemInventoryUnitCost = itm.ItemInventoryUnitCost
            };
            return it;
        }
        public EstimateDetailModel GetEstimateDataById(int id)
        {
            var itm = _context.tblEstimateDetails.Find(id);
            var it = new EstimateDetailModel
            {
                ItemInventoryID = itm.ItemInventoryID,
                EstimateDetailID = itm.EstimateDetailID,
                ItemCategoryID = GetCategoryByEstId(itm.EstimateDetailID),
                EstimateDetailCostCodeGL = itm.EstimateDetailCostCodeGL,
                NonInventoryItem = itm.NonInventoryItem,
                EstimateDetailLineItemTotal = itm.EstimateDetailLineItemTotal,
                EstimateID = itm.EstimateID,
                EstimateDetailQuantity = itm.EstimateDetailQuantity,
                EstimateDescription = itm.EstimateDescription,
                ItemInventoryCost = itm.ItemInventoryCost
            };
            return it;
        }
        public InvoiceModel GetInvoiceDetails(int InvoiceId)
        {
            var model = _context.tblInvoices.SingleOrDefault(p => p.InvoiceID == InvoiceId);
            var im = MapInvoice(model);
            return im;
        }

        private InvoiceModel MapInvoice(tblInvoice model)
        {
            InvoiceModel im = new InvoiceModel();
            im.InvoiceID = model.InvoiceID;
            im.ClientTypeID = model.ClientTypeID;
            im.LocationID = model.LocationID.HasValue ? model.LocationID.Value : 0;
            im.InvoiceJobNumber = model.InvoiceJobNumber;
            im.InvoiceNotes = model.InvoiceNotes;
            //im.InvoiceTerms = model.InvoiceTerms;
            im.DefaultValue = model.InvoiceTerms;
            im.InvoiceTotal = model.InvoiceTotal;
            im.InvoiceVendor = model.VendorInvoice_;
            im.InvoiceStatus = model.InvoiceStatus;
            im.InvoiceDate = model.InvoiceDate;
            im.InvoiceNumber = model.InvoiceNumber;
            //im.Employee = model.EmployeeID.ToString();
            if (model.InvoiceJobNumber == "NULL" || model.InvoiceJobNumber == null) {
                im.Employee = GetEmployeeDtlsById(model.EmployeeID.Value).FirstName + " " + GetEmployeeDtlsById(model.EmployeeID.Value).LastName;
                im.employID = model.EmployeeID.Value;
            }
            im.CreatedDate = model.CreatedDate;
            im.CreatedUser = model.CreatedUser;
            im.Vendor = model.Vendor_;
            im.InvoiceDetails = model.tblInvoiceDetails.Select(p => new InvoiceDetailModel
            {
                InvoiceDetailID = p.InvoiceDetailID,
                ItemInventoryID = p.ItemInventoryID,
                ItemInventoryDescription = p.ItemInventoryDescription,
                ItemInventoryUnitCost = p.ItemInventoryUnitCost,
                NonInventoryItem = p.NonInventoryItem,
                InvoiceDetailQuantity = p.InvoiceDetailQuantity,
                InvoiceDetailLineItemTotal = p.InvoiceDetailLineItemTotal,
                InvoiceDetailCostCodeGL = p.InvoiceDetailCostCodeGL

            }).ToList();
            return im;
        }
        public EstimateModel GetEstimateDetails(int EstimateId)
        {
            var model = _context.tblEstimates.SingleOrDefault(p => p.EstimateId == EstimateId);
            var im = MapEstimate(model);
            return im;
        }

        private EstimateModel MapEstimate(tblEstimate model)
        {
            EstimateModel im = new EstimateModel();
            im.Estimate = model.EstimateId;
            im.ClientTypeID = model.ClientTypeID;
            im.LocationID = model.LocationID.HasValue ? model.LocationID.Value : 0;
            im.InvoiceJobNumber = model.ClientName;
            im.EstimateNotes = model.EstimateNotes;
            im.DefaultValue = model.EstimateTerms;
            im.Total = model.Total;
            im.EstimateVendor = model.VendoeEstimate;
            im.Status = model.Status;
            im.EstimateDate = model.EstimateDate;
            im.EstimateNumber = model.EstimateNumber;
            im.Employee = model.EmployeeID.ToString();
            im.CreatedDate = model.CreatedDate;
            im.CreatedUser = model.CreatedUser;
            im.Vendor = model.Vendor_;
            im.EstimateDetails = model.tblEstimateDetails.Select(p => new EstimateDetailModel
            {
                EstimateID = p.EstimateID,
                EstimateDetailID = p.EstimateDetailID,
                ItemInventoryID = p.ItemInventoryID,
                EstimateDescription = p.EstimateDescription,
                ItemInventoryCost = p.ItemInventoryCost,
                NonInventoryItem = p.NonInventoryItem,
                EstimateDetailQuantity = p.EstimateDetailQuantity,
                EstimateDetailLineItemTotal = p.EstimateDetailLineItemTotal,
                EstimateDetailCostCodeGL = p.EstimateDetailCostCodeGL
            }).ToList();
            return im;
        }
        public InvoiceResult SaveInvDtls(InvoiceModel model)
        {
            tblInvoice In = new tblInvoice();
            In.ClientTypeID = model.ClientTypeID;
            //In.InvoiceID = model.InvoiceID;
            In.InvoiceJobNumber = model.InvoiceJobNumber;
            In.InvoiceNumber = "123";
            In.EmployeeID = model.ClientTypeID == 1 ? Convert.ToInt32(model.Employee) : (int?)null;
            In.Vendor_ = model.Vendor;
            In.LocationID = model.LocationID;
            In.InvoiceStatus = model.InvoiceStatus == "D" ? "Drafted" : model.InvoiceStatus == "ME" ? "Drafted" : "Completed";
            In.InvoiceTerms = model.DefaultValue;
            In.ExternalClientID = model.ExternalClientID;
            In.InvoiceTotal = model.InvoiceTotal;
            In.InvoiceDate = model.InvoiceDate;
            In.InvoiceNotes = model.InvoiceNotes;
            In.VendorInvoice_ = model.InvoiceVendor;
            In.CreatedUser = model.CreatedUser;
            In.CreatedDate = DateTime.Now;
            //save InoviceDetails
            if (model.InvoiceDetails != null && model.InvoiceDetails.Any())
            {
                foreach (var item in model.InvoiceDetails)
                {
                    var idt = new tblInvoiceDetail
                    {
                        InvoiceDetailID = item.InvoiceDetailID,
                        ItemInventoryID = item.ItemInventoryID,
                        NonInventoryItem = item.NonInventoryItem,
                        ItemInventoryDescription = item.ItemInventoryDescription,
                        ItemInventoryUnitCost = item.ItemInventoryUnitCost,
                        InvoiceDetailCostCodeGL = item.InvoiceDetailCostCodeGL,
                        InvoiceDetailLineItemTotal = item.InvoiceDetailLineItemTotal,
                        InvoiceDetailQuantity = item.InvoiceDetailQuantity,
                        InvoiceID = item.InvoiceID
                    };
                    In.tblInvoiceDetails.Add(idt);
                    var ct = model.InvoiceDetails.Count();
                    //if (model.InvoiceStatus != "D" && model.InvoiceDetails.FirstOrDefault().NonInventoryItem == null)
                    //if ((model.InvoiceStatus == "D" || model.InvoiceStatus == "ME" || model.InvoiceStatus == "A") && item.NonInventoryItem == null)
                    if ((model.InvoiceStatus == "A") && item.NonInventoryItem == null)
                    {
                        var checkQty = false;
                        var itm = _context.tblItemInventories.SingleOrDefault(x => x.ItemInventoryID == item.ItemInventoryID);
                        if (itm.ItemInventoryQuantity >= item.InvoiceDetailQuantity.Value)
                        {
                            itm.ItemInventoryQuantity = itm.ItemInventoryQuantity - item.InvoiceDetailQuantity.Value;
                            checkQty = true;
                        }
                        if (!checkQty)
                        {
                            return new InvoiceResult { InvoiceId = -1, Inventory = item.ItemInventoryDescription, Status = false };
                        }
                    }
                }
            }
            _context.tblInvoices.Add(In);
            var r = _context.SaveChanges();
            return new InvoiceResult { InvoiceId = In.InvoiceID, Inventory = "", Status = true };
        }
        public InvoiceResult SaveEstimateDtls(EstimateModel model)
        {
            tblEstimate es = new tblEstimate();
            es.EstimateNumber = "123";
            es.ClientName = model.InvoiceJobNumber;
            es.CreatedDate = DateTime.Now;
            es.Total = model.Total;
            es.ClientName = model.InvoiceJobNumber;
            es.Status = model.Status == "D" ? "Drafted" : model.Status == "ME" ? "Drafted" : "";
            es.Vendor_ = model.Vendor;
            es.VendoeEstimate = model.EstimateVendor;
            es.CreatedUser = model.CreatedUser;
            es.EstimateTerms = model.DefaultValue;
            es.LocationID = model.LocationID;
            es.EstimateDate = model.EstimateDate;
            es.ExternalClientID = model.ExternalClientID;
            es.EstimateNotes = model.EstimateNotes;
            es.ClientTypeID = model.ClientTypeID;
            es.EmployeeID = model.ClientTypeID == 1 ? Convert.ToInt32(model.Employee) : (int?)null;


            if (model.EstimateDetails != null && model.EstimateDetails.Any())
            {
                foreach (var item in model.EstimateDetails)
                {
                    var idt = new tblEstimateDetail
                    {
                        EstimateDetailID = item.EstimateDetailID,
                        ItemInventoryID = item.ItemInventoryID,
                        NonInventoryItem = item.NonInventoryItem,
                        EstimateDescription = item.EstimateDescription,
                        ItemInventoryCost = item.ItemInventoryCost,
                        EstimateDetailCostCodeGL = item.EstimateDetailCostCodeGL,
                        EstimateDetailLineItemTotal = item.EstimateDetailLineItemTotal,
                        EstimateDetailQuantity = item.EstimateDetailQuantity,
                        EstimateID = item.EstimateID
                    };
                    es.tblEstimateDetails.Add(idt);
                    var ct = model.EstimateDetails.Count();
                    if ((model.Status == "D" || model.Status == "ME") && item.NonInventoryItem == null)
                    {
                        //var checkQty = false;
                        var itm = _context.tblItemInventories.SingleOrDefault(x => x.ItemInventoryID == item.ItemInventoryID);
                        //if (itm.ItemInventoryQuantity >= item.InvoiceDetailQuantity.Value)
                        //{
                        //    itm.ItemInventoryQuantity = itm.ItemInventoryQuantity - item.InvoiceDetailQuantity.Value;
                        //    checkQty = true;
                        //}
                        //if (!checkQty)
                        //{
                        //    return new InvoiceResult { InvoiceId = -1, Inventory = item.EstimateDescription, Status = false };
                        //}
                    }
                }
            }
            _context.tblEstimates.Add(es);
            var r = _context.SaveChanges();
            return new InvoiceResult { InvoiceId = es.EstimateId, Inventory = "", Status = true };
        }
        public bool SaveInvoiceItem(int InvoiceId, InvoiceDetailModel item)
        {
            var invoice = _context.tblInvoices.Find(InvoiceId);
            var idt = new tblInvoiceDetail
            {
                InvoiceDetailID = item.InvoiceDetailID,
                ItemInventoryID = item.ItemInventoryID,
                ItemInventoryDescription = item.ItemInventoryDescription,
                ItemInventoryUnitCost = item.ItemInventoryUnitCost,
                NonInventoryItem = item.NonInventoryItem,
                InvoiceDetailCostCodeGL = item.InvoiceDetailCostCodeGL,
                InvoiceDetailLineItemTotal = item.InvoiceDetailLineItemTotal,
                InvoiceDetailQuantity = item.InvoiceDetailQuantity,
                InvoiceID = InvoiceId
            };
            invoice.tblInvoiceDetails.Add(idt);
            invoice.InvoiceTotal = invoice.InvoiceTotal + item.InvoiceDetailLineItemTotal;
            var r = _context.SaveChanges();
            return r > 0;
        }
        public bool SaveEstimateItem(int estimateId, EstimateDetailModel item)
        {
            var invoice = _context.tblEstimates.Find(estimateId);
            var idt = new tblEstimateDetail
            {
                EstimateDetailID = item.EstimateDetailID,
                ItemInventoryID = item.ItemInventoryID,
                EstimateDescription = item.EstimateDescription,
                ItemInventoryCost = item.ItemInventoryCost,
                NonInventoryItem = item.NonInventoryItem,
                EstimateDetailCostCodeGL = item.EstimateDetailCostCodeGL,
                EstimateDetailLineItemTotal = item.EstimateDetailLineItemTotal,
                EstimateDetailQuantity = item.EstimateDetailQuantity,
                EstimateID = estimateId
            };
            invoice.tblEstimateDetails.Add(idt);
            invoice.Total = invoice.Total + item.EstimateDetailLineItemTotal;
            var r = _context.SaveChanges();
            return r > 0;
        }
        public int SaveEditItmProd(ItemInventoryModel model)
        {
            tblItemInventory it;
            if (model.ItemInventoryID > 0)
            {
                it = _context.tblItemInventories.Find(model.ItemInventoryID);
                it.ItemInventoryDescription = model.ItemInventoryDescription;
                it.ItemInventoryReplacementCost = model.ItemInventoryReplacementCost;
                it.ItemInventoryNumber = model.ItemInventoryNumber;
                it.ModDate = DateTime.Now;
                it.ModUser = model.CreatedUser;
            }
            else
            {
                it = new tblItemInventory
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = model.CreatedUser
                };
                _context.tblItemInventories.Add(it);
            }
            it.ItemCategoryID = model.ItemCategoryID;
            it.ItemInventoryID = model.ItemInventoryID;
            it.ItemInventoryQuantity = model.ItemInventoryQuantity;
            it.ItemInventorySalesPrice = model.ItemInventorySalesPrice;
            it.ItemInventoryReplacementCost = model.ItemInventoryReplacementCost;
            it.ItemInventoryNumber = model.ItemInventoryNumber;
            it.ItemInventoryDescription = model.ItemInventoryDescription;
            it.LocationID = model.LocationID;
            var r = _context.SaveChanges();
            return it.ItemInventoryID;
        }
        //save for edit invoices
        public InvoiceModel SaveEditInvoicesDetails(InvoiceDetailModel model)
        {
            tblInvoiceDetail id;

            var invoice = _context.tblInvoices.Find(model.InvoiceID);
            id = _context.tblInvoiceDetails.Find(model.InvoiceDetailID);
            var oldtotal = id.InvoiceDetailLineItemTotal;
            id.InvoiceDetailID = model.InvoiceDetailID;
            id.InvoiceDetailCostCodeGL = model.InvoiceDetailCostCodeGL;
            id.InvoiceDetailQuantity = model.InvoiceDetailQuantity;
            id.ItemInventoryDescription = model.ItemInventoryDescription;
            id.NonInventoryItem = model.NonInventoryItem;
            id.ItemInventoryUnitCost = model.ItemInventoryUnitCost;
            id.InvoiceDetailLineItemTotal = model.InvoiceDetailLineItemTotal;
            id.ItemInventoryID = model.ItemInventoryID;
            //since iteminventory id is not passing its throwing sql exception
            if (oldtotal != model.InvoiceDetailLineItemTotal)
            {
                invoice.InvoiceTotal = invoice.tblInvoiceDetails.Sum(j => j.InvoiceDetailLineItemTotal);
            }
            var r = _context.SaveChanges();
            return MapInvoice(invoice);
        }
        //save for edit estimates
        public EstimateModel SaveEditEstimateDetails(EstimateDetailModel model)
        {
            tblEstimateDetail id;

            var estimate = _context.tblEstimates.Find(model.EstimateID);
            id = _context.tblEstimateDetails.Find(model.EstimateDetailID);
            var oldtotal = id.EstimateDetailLineItemTotal;
            id.EstimateDetailID = model.EstimateDetailID;
            id.EstimateDetailCostCodeGL = model.EstimateDetailCostCodeGL;
            id.EstimateDetailQuantity = model.EstimateDetailQuantity;
            id.EstimateDescription = model.EstimateDescription;
            id.NonInventoryItem = model.NonInventoryItem;
            id.ItemInventoryCost = model.ItemInventoryCost;
            id.EstimateDetailLineItemTotal = model.EstimateDetailLineItemTotal;
            id.ItemInventoryID = model.ItemInventoryID;
            //since iteminventory id is not passing its throwing sql exception
            if (oldtotal != model.EstimateDetailLineItemTotal)
            {
                estimate.Total = estimate.tblEstimateDetails.Sum(j => j.EstimateDetailLineItemTotal);
            }
            var r = _context.SaveChanges();
            return MapEstimate(estimate);
        }
        public InvoiceResult updateInvoiceDetail(InvoiceModel model)
        {
            if (model.InvoiceJobNumber == null)
            {
                var emp = GetEmployeeDtlsById(model.employID);
            }
            tblInvoice tI;
            var inv = _context.tblInvoices.Find(model.InvoiceID);
            inv.ClientTypeID = model.ClientTypeID;
            if (model.InvoiceJobNumber == null)
            {
                inv.EmployeeID = model.employID;
            }
            inv.ExternalClientID = model.ExternalClientID;
            inv.InvoiceJobNumber = model.InvoiceJobNumber;
            inv.LocationID = model.LocationID;
            inv.InvoiceTerms = model.DefaultValue;
            inv.InvoiceNotes = model.InvoiceNotes;
            inv.InvoiceNotes = model.InvoiceNotes;
            inv.InvoiceDate = model.InvoiceDate;
            inv.VendorInvoice_ = model.InvoiceVendor;
            inv.Vendor_ = model.Vendor;
            
            inv.InvoiceStatus = model.InvoiceStatus == "D" ? "Drafted" : model.InvoiceStatus == "ME" ? "Drafted" : "Completed";
            //inv.InvoiceStatus = model.InvoiceStatus == "D" ? "Drafted" : "Completed";
            //inv.InvoiceTotal = model.tblInvoiceDetails.Sum(j => j.InvoiceDetailLineItemTotal);
           //var invd = _context.tblInvoiceDetails.Find(model.InvoiceID);
            
            if (inv.InvoiceStatus == "Completed")
                {
                foreach (var item in inv.tblInvoiceDetails)
                {
                    if (item.NonInventoryItem==null) {
                        var checkQty = false;
                        var itm = _context.tblItemInventories.SingleOrDefault(x => x.ItemInventoryID == item.ItemInventoryID);
                        if (itm.ItemInventoryQuantity >= item.InvoiceDetailQuantity.Value)
                        {
                            itm.ItemInventoryQuantity = itm.ItemInventoryQuantity - item.InvoiceDetailQuantity.Value;
                            checkQty = true;
                        }
                        if (!checkQty)
                        {
                            return new InvoiceResult { InvoiceId = -1, Inventory = item.ItemInventoryDescription, Status = false };
                        }
                    }
                }
                }            
            var s = _context.SaveChanges() > 0;
            return  new InvoiceResult { InvoiceId = model.InvoiceID ,Status = true };
        }
        public tblEstimate updateEstimateDetail(EstimateModel model)
        {
            tblEstimate tI;
            var inv = _context.tblEstimates.Find(model.Estimate);
            inv.ClientTypeID = model.ClientTypeID;
            //inv.EmployeeID =model.Employee = "NULL" ? "NULL":Convert.ToInt32(model.Employee);
            inv.ExternalClientID = model.ExternalClientID;
            inv.ClientName = model.InvoiceJobNumber;
            inv.LocationID = model.LocationID;
            inv.EstimateTerms = model.DefaultValue;
            inv.EstimateNotes = model.EstimateNotes;
            inv.EstimateDate = model.EstimateDate;
            inv.VendoeEstimate = model.EstimateVendor;
            inv.Vendor_ = model.Vendor;
            inv.Status = model.Status == "D" ? "Drafted" : model.Status == "ME" ? "Drafted" : "Completed";
            //inv.InvoiceStatus = model.InvoiceStatus == "D" ? "Drafted" : "Completed";
            //inv.InvoiceTotal = model.tblInvoiceDetails.Sum(j => j.InvoiceDetailLineItemTotal);
            var s = _context.SaveChanges() > 0;
            return inv;
        }
        public IList<InvoiceModel> GetInvoiceList()
        {
            var Invlst = _context.tblInvoices.Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                LocationID = p.LocationID.HasValue ? p.LocationID.Value : 0,
                InvoiceJobNumber = p.InvoiceJobNumber,
                Vendor = p.Vendor_,
                InvoiceTotal = p.InvoiceTotal,
                DefaultValue = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceStatus = p.InvoiceStatus,
                InvoiceVendor = p.VendorInvoice_,
                CreatedDate = DateTime.Now,
                CreatedUser = p.CreatedUser
            }).ToList();
            return Invlst;
        }
        public IList<InvoiceModel> GetArchivedInvoiceList()
        {
            var Invlst = _context.tblInvoices.Where(k => k.InvoiceStatus == "Archived").Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                LocationID = p.LocationID.HasValue ? p.LocationID.Value : 0,
                InvoiceJobNumber = p.InvoiceJobNumber,
                Vendor = p.Vendor_,
                InvoiceTotal = p.InvoiceTotal,
                DefaultValue = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceStatus = p.InvoiceStatus,
                InvoiceVendor = p.VendorInvoice_,
                CreatedDate = DateTime.Now,
                CreatedUser = p.CreatedUser
            }).ToList();
            return Invlst;
        }
        public static string GetUserFullname(string userName)
        {
            var info = UserUtils.FindUserInfo(userName.Trim());
            return info.DisplayName;
        }
        public List<InvoicegData> getInvoiceDashboarddata()
        {
            var grouped = (from p in _context.tblInvoices
                           group p by new { month = p.CreatedDate.Month, year = p.CreatedDate.Year } into d
                           select new InvoicegData { Month = d.Key.month, Year = d.Key.year, Completed = d.Where(p => p.InvoiceStatus == "Completed" || p.InvoiceStatus == "Archived").Sum(k => k.InvoiceTotal) ?? 0, Drafted = d.Where(p => p.InvoiceStatus == "Drafted").Sum(k => k.InvoiceTotal) ?? 0 }).ToList();
            return grouped;
        }
        public IList<InvoiceModel> GetInvoiceLists()
        {
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var dt = DateTime.Now.AddDays(-days);
            var Invlst = _context.tblInvoices.Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                LocationID = p.LocationID.HasValue ? p.LocationID.Value : 0,
                InvoiceJobNumber = p.InvoiceJobNumber,
                Vendor = p.Vendor_,
                InvoiceTotal = p.InvoiceTotal,
                DefaultValue = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceVendor = p.VendorInvoice_,
                InvoiceStatus = p.InvoiceStatus,
                CreatedDate = DateTime.Now,
                CreatedUser = p.CreatedUser
            }).Where(x => x.InvoiceDate > dt).ToList();
            return Invlst;
        }
        public List<EstimateModel> GetEstimatesList()
        {
            var Invlst = _context.tblEstimates.Select(p => new EstimateModel
            {
                Estimate = p.EstimateId,
                EstimateNumber = p.EstimateNumber,
                InvoiceJobNumber = p.ClientName,
                CreatedDate = p.CreatedDate,
                Status = p.Status,
                Total = p.Total
            }).ToList();
            return Invlst;
        }
        public List<CllientModel> GetClient()
        {
            var lst = _context.tblClientTypes.Select(p => new CllientModel
            {
                ClientTypeID = p.ClientTypeID,
                ClientTypeDescription = p.ClientTypeDescription,
            }).ToList();
            return lst;
        }
        public List<EmployeeModel> GetEmployeeDetails()
        {
            var emp = _context.tblEmployees.Select(p => new EmployeeModel
            {
                EmployeeID = p.EmployeeID,
                Email = p.Email,
                FirstName = p.FirstName,
                LastName = p.LastName,
                EmpCode = p.EmpCode,
                Active = p.Active

            }).ToList();
            return emp;
        }
        public EmployeeModel GetEmployeeDtlsById(int id)
        {
            var itm = _context.tblEmployees.Find(id);
            var it = new EmployeeModel
            {
                Active = itm.Active,
                Email = itm.Email,
                EmpCode = itm.EmpCode,
                EmployeeID = itm.EmployeeID,
                FirstName = itm.FirstName,
                LastName = itm.LastName,
            };
            return it;
        }
        public EmployeeModel GetEmployeeDtlsByemail(string email)
        {
            var itm = _context.tblEmployees.Find(email);
            var it = new EmployeeModel
            {
                Active = itm.Active,
                Email = itm.Email,
                EmpCode = itm.EmpCode,
                EmployeeID = itm.EmployeeID,
                FirstName = itm.FirstName,
                LastName = itm.LastName,
            };
            return it;
        }
        public List<JobModel> GetJobDetails()
        {
            var jlst = _context.tblJobs.Select(p => new JobModel
            {
                Job = p.Job,
                JobDescription = p.Job + " " + p.JobDescription,
                MailAddress = p.MailAddress,
                MailCity = p.MailCity,
                MailState = p.MailState,
                MailZip = p.MailZip,
                ShipAddress = p.ShipAddress,
                ShipCity = p.ShipCity,
                ShipState = p.ShipState,
                ShipZip = p.ShipZip,
                Active = p.Active

            }).ToList();
            return jlst.OrderByDescending(p => p.Job).ToList();
        }
        public JobModel GetJobDetails(string id)
        {
            var jlst = _context.tblJobs.Find(id);
            var lst = new JobModel
            {
                Job = jlst.Job,
                JobDescription = jlst.Job + " " + jlst.JobDescription,
                JCCo = jlst.JCCo.Value,
                MailAddress = jlst.MailAddress,
                MailCity = jlst.MailCity,
                MailState = jlst.MailState,
                MailZip = jlst.MailZip,
                ShipAddress = jlst.ShipAddress,
                ShipCity = jlst.ShipCity,
                ShipState = jlst.ShipState,
                ShipZip = jlst.ShipZip,
                Active = jlst.Active

            };
            return lst;
        }
        public string GetEmpLocation(string email)
        {
            String loc = "All";
            var lst = (from e in _context.tblEmployees
                       join el in _context.tblEmployeeLocations on e.EmployeeID equals el.EmployeeID
                       where e.Email == email
                       select new EmployeeLocation
                       {
                           EmployeeID = e.EmployeeID,
                           email = e.Email,
                           LocationID = el.LocationID,
                           UseAllAsDefault = el.UseAllAsDefault
                       });

            var emp = lst.FirstOrDefault();
            if (emp != null && !emp.UseAllAsDefault)
                loc = emp.LocationID == 1 ? "Bessemer" : "Houston";

            return loc;
        }
        public IList<ExternalClientModel> GetExternalClientList()
        {
            var clst = _context.tblExternalClients.Select(p => new ExternalClientModel
            {
                ExternalClientID = p.ExternalClientID,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Employer = p.Employer,
                City = p.City,
                Address = p.Address,
                State = p.State,
                Zip = p.Zip
            }).ToList();
            return clst.OrderBy(p => p.FirstName).ToList();
        }

        public int SaveExternalClientdtls(ExternalClientModel model)
        {
            tblExternalClient ec = new tblExternalClient();
            ec.ExternalClientID = model.ExternalClientID;
            ec.FirstName = model.FirstName;
            ec.LastName = model.LastName;
            ec.State = model.State;
            ec.Address = model.Address;
            ec.Employer = model.Employer;
            ec.Email = model.Email;
            ec.City = model.City;
            ec.Zip = model.Zip;
            _context.tblExternalClients.Add(ec);
            var r = _context.SaveChanges();
            return ec.ExternalClientID;
        }

        public List<ItemInventoryModel> GetSubItemInventory(int CategoryId)
        {
            var lst = _context.tblItemInventories.Where(k => k.ItemCategoryID == CategoryId).Select(p => new ItemInventoryModel
            {
                ItemInventoryID = p.ItemInventoryID,
                ItemInventoryNumber = p.ItemInventoryNumber,
                ItemInventoryDescription = p.ItemInventoryDescription,
                ItemCategoryID = p.ItemCategoryID,
                ItemInventoryQuantity = p.ItemInventoryQuantity,
                ItemInventoryReorderPoint = p.ItemInventoryReorderPoint,
                ItemInventoryExtendedCost = p.ItemInventoryExtendedCost,
                ItemInventorySalesPrice = p.ItemInventorySalesPrice,
                LocationID = p.LocationID,
                ItemInventoryMarkup = p.ItemInventoryMarkup
            }).Where(x=>x.ItemCategoryID!=23).ToList();
            return lst;
        }
        public InvoiceModel UpdateInvoiceStatus(int Id, string InvStatus)
        {
            tblInvoice emodel;

            using (var c = new WISEntities())
            {
                emodel = c.tblInvoices.SingleOrDefault(p => p.InvoiceID == Id);
                emodel.InvoiceStatus = InvStatus == "D" ? "Drafted" : InvStatus == "ME" ? "Drafted" : "Completed";
                
                c.SaveChanges();
               
            }

            return new InvoiceModel { InvoiceStatus = emodel.InvoiceStatus, InvoiceID = Id };
        }

        public int UpdateEstimateStatus(int estimateID)
        {
            int result;
            using (var c = new WISEntities())
            {
                var lst = c.tblEstimates.Where(p => p.EstimateId == estimateID);
                foreach (var emodel in lst)
                {
                    emodel.Status = "accepted";
                }
                result = c.SaveChanges();
            }
            return result;
        }
        public int ConvEsttoInv(int estimateID)
        {
            tblEstimate es = _context.tblEstimates.Find(estimateID);
            tblInvoice In = new tblInvoice();
            In.ClientTypeID = es.ClientTypeID;
            In.InvoiceJobNumber = es.ClientName;
            In.InvoiceNumber = "123";
            In.EmployeeID = es.ClientTypeID == 1 ? Convert.ToInt32(es.EmployeeID) : (int?)null;
            In.Vendor_ = es.Vendor_;
            In.LocationID = es.LocationID;
            In.InvoiceStatus = "Drafted";
            In.InvoiceTerms = es.EstimateTerms;
            In.ExternalClientID = es.ExternalClientID;
            In.InvoiceTotal = es.Total;
            In.InvoiceDate = es.EstimateDate;
            In.InvoiceNotes = es.EstimateNotes;
            In.VendorInvoice_ = es.VendoeEstimate;
            In.CreatedUser = es.CreatedUser;
            In.CreatedDate = DateTime.Now;
            _context.tblInvoices.Add(In);
            var lst = GetEstimateDetails(estimateID);
            tblEstimateDetail ed = _context.tblEstimateDetails.Find(estimateID);
            foreach (var item in lst.EstimateDetails)
            {
                var Id = new tblInvoiceDetail()
                {
                    InvoiceDetailID = item.EstimateDetailID,
                    ItemInventoryID = item.ItemInventoryID,
                    NonInventoryItem = item.NonInventoryItem,
                    ItemInventoryDescription = item.EstimateDescription,
                    ItemInventoryUnitCost = item.ItemInventoryCost,
                    InvoiceDetailCostCodeGL = item.EstimateDetailCostCodeGL,
                    InvoiceDetailLineItemTotal = item.EstimateDetailLineItemTotal,
                    InvoiceDetailQuantity = item.EstimateDetailQuantity
                };
                _context.tblInvoiceDetails.Add(Id);
            }
            return _context.SaveChanges();
        }

        public int BulkUpdateInvoiceStatus(int invoice)
        {
            int result;
            using (var c = new WISEntities())
            {
                var lst = c.tblInvoices.Where(p => p.InvoiceID == invoice);
                foreach (var emodel in lst)
                {
                    emodel.IsArchived = true;
                    emodel.InvoiceStatus = "Archived";
                }

                result = c.SaveChanges();

            }
            return result;
        }

        public IList<InvoiceModel> GetGLCodesInvoiceList()
        {
            var Invlst = _context.tblInvoices.Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                LocationID = p.LocationID.HasValue ? p.LocationID.Value : 0,
                InvoiceJobNumber = p.InvoiceJobNumber,
                Vendor = p.Vendor_,
                InvoiceTotal = p.InvoiceTotal,
                DefaultValue = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceVendor = p.VendorInvoice_,
                InvoiceStatus = p.InvoiceStatus,
                CreatedDate = DateTime.Now,
                CreatedUser = p.CreatedUser,
            });
            return Invlst.ToList();
        }
       
        public IList<InvoiceDetailModel> GetGLInvoiceDetails(string gl)
        {
            gl = "CW29200.000.000070";
            var itm = _context.tblInvoiceDetails.Where(k => k.InvoiceDetailCostCodeGL == gl).Select(p => new InvoiceDetailModel
              {
                ItemInventoryID = p.ItemInventoryID,
                InvoiceDetailID = p.InvoiceDetailID,
                //ItemCategoryID = GetCategoryById(p.InvoiceDetailID),
                InvoiceDetailCostCodeGL = p.InvoiceDetailCostCodeGL,
                NonInventoryItem = p.NonInventoryItem,
                InvoiceDetailLineItemTotal = p.InvoiceDetailLineItemTotal,
                InvoiceID = p.InvoiceID,
                InvoiceDetailQuantity = p.InvoiceDetailQuantity,
                ItemInventoryDescription = p.ItemInventoryDescription,
                ItemInventoryUnitCost = p.ItemInventoryUnitCost
            }).ToList();
            return itm;
        }       
        public IList<InvoiceDetailModel> GetGL1InvoiceDetails(string gl)
        {
            gl = "CB29200.000.000030";
            var itm = _context.tblInvoiceDetails.Where(k => k.InvoiceDetailCostCodeGL == gl).Select(p => new InvoiceDetailModel
            {
                ItemInventoryID = p.ItemInventoryID,
                InvoiceDetailID = p.InvoiceDetailID,
                //ItemCategoryID = GetCategoryById(p.InvoiceDetailID),
                InvoiceDetailCostCodeGL = p.InvoiceDetailCostCodeGL,
                NonInventoryItem = p.NonInventoryItem,
                InvoiceDetailLineItemTotal = p.InvoiceDetailLineItemTotal,
                InvoiceID = p.InvoiceID,
                InvoiceDetailQuantity = p.InvoiceDetailQuantity,
                ItemInventoryDescription = p.ItemInventoryDescription,
                ItemInventoryUnitCost = p.ItemInventoryUnitCost
            }).ToList();
            return itm;
        }

//        SELECT* from[dbo].[tblInvoice] as Inv

//INNER JOIN[dbo].[tblInvoiceDetail] as Indt ON Inv.InvoiceID=Indt.InvoiceID
//where   InvoiceDetailCostCodeGL='CW29200.000.000070';

        public IList<InvoiceModel> GetGL1InvoiceSummary(string GL)
        {
            GL = "CW29200.000.000070";
            var lst = (from id in _context.tblInvoiceDetails
                       join Inv in _context.tblInvoices on id.InvoiceID equals Inv.InvoiceID
                       where id.InvoiceDetailCostCodeGL == GL
                       select new InvoiceModel
                       {
                           InvoiceID =Inv.InvoiceID,
                           InvoiceDate=Inv.InvoiceDate,
                           InvoiceVendor=Inv.VendorInvoice_,
                           InvoiceJobNumber=Inv.InvoiceJobNumber,
                           InvoiceNotes=Inv.InvoiceNotes,
                           InvoiceStatus=Inv.InvoiceStatus,
                           InvoiceNumber=Inv.InvoiceNumber,InvoiceTotal=Inv.InvoiceTotal,
                           DefaultValue=Inv.InvoiceNotes,
                           TotalAmount=Inv.InvoiceTotal,
                           Vendor=Inv.Vendor_,
                           IsArchived=Inv.IsArchived.Value
                       });
            return lst.ToList();
        }
        public IList<InvoiceModel> GetGLInvoiceSummary(string GL)
        {
            GL = "CB29200.000.000030";
            var lst = (from id in _context.tblInvoiceDetails
                       join Inv in _context.tblInvoices on id.InvoiceID equals Inv.InvoiceID
                       where id.InvoiceDetailCostCodeGL == GL
                       select new InvoiceModel
                       {
                           InvoiceID = Inv.InvoiceID,
                           InvoiceDate = Inv.InvoiceDate,
                           InvoiceVendor = Inv.VendorInvoice_,
                           InvoiceJobNumber = Inv.InvoiceJobNumber,
                           InvoiceNotes = Inv.InvoiceNotes,
                           InvoiceStatus = Inv.InvoiceStatus,
                           InvoiceNumber = Inv.InvoiceNumber,
                           InvoiceTotal = Inv.InvoiceTotal,
                           DefaultValue = Inv.InvoiceNotes,
                           TotalAmount = Inv.InvoiceTotal,
                           Vendor = Inv.Vendor_,
                           IsArchived = Inv.IsArchived.Value
                       });
            return lst.ToList();
        }
    }    
}

