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
    public class WISService: IWISService
    {
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
                        select new ItemInventoryModel {
                        ItemInventoryID=ti.ItemInventoryID,
                        ItemInventoryNumber = ti.ItemInventoryNumber,
                        ItemInventoryDescription =ti.ItemInventoryDescription,                        
                        ItemCategoryID = ti.ItemCategoryID,
                        ItemInventoryQuantity=ti.ItemInventoryQuantity,
                        LocationID= ti.LocationID,
                        LocationDescription = loc.LocationDescription,
                        ItemInventoryReorderPoint=ti.ItemInventoryReorderPoint,
                        ItemInventoryReplacementCost=Math.Round(ti.ItemInventoryReplacementCost??0,2),
                        ItemInventoryMarkup=ti.ItemInventoryMarkup,
                        ItemInventorySalesPrice= Math.Round(ti.ItemInventorySalesPrice??0,2),
                        ItemInventoryReplacementCostOnHand =ti.ItemInventoryReplacementCostOnHand,
                        ItemInventoryExtendedCost = Math.Round(ti.ItemInventoryExtendedCost??0,2),
                        CreatedDate=ti.CreatedDate,
                        CreatedUser=ti.CreatedUser,
                        ModDate=ti.ModDate,
                        ModUser=ti.ModUser 
            }).ToList();
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

       

        public List<ItemCategory> GetItemCategories()
        {
            var lst = _context.tblItemCategories.Select(p => new ItemCategory
            {
                 ItemCategoryID= p.ItemCategoryID,
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
           var res= lst.SingleOrDefault();
            if (res != null)
                return res.ItemCategoryID.Value;
            else
                return 0;

        }

        public List<LocationModel> GetLocationList()
        {
          
            var lst = _context.tblLocations.Select(p => new LocationModel
            {
                LocationID = p.LocationID,
                LocationDescription = p.LocationDescription,
              
            }).ToList();
            return lst;
        }

        public ItemInventoryModel GetInventoryDataById( int id)
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

        public InvoiceModel DeleteInvoiceDetail(int invoiceDetailID,int invoiceId)
        {
            var invoiceitm = _context.tblInvoices.Find(invoiceId);
            var itm = invoiceitm.tblInvoiceDetails.SingleOrDefault(p => p.InvoiceDetailID == invoiceDetailID);
            //itm.InvoiceTotal = itm.InvoiceTotal - itm.tblInvoiceDetails.FirstOrDefault().InvoiceDetailLineItemTotal;
            _context.tblInvoiceDetails.Remove(itm);
            _context.SaveChanges() ;
            invoiceitm.InvoiceTotal = invoiceitm.tblInvoiceDetails.Sum(j => j.InvoiceDetailLineItemTotal);
            var s = _context.SaveChanges() > 0;
            var im = MapInvoice(invoiceitm);
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
                InvoiceDetailCostCodeGL =itm.InvoiceDetailCostCodeGL,
                NonInventoryItem=itm.NonInventoryItem,
                InvoiceDetailLineItemTotal=itm.InvoiceDetailLineItemTotal,
                InvoiceID=itm.InvoiceID,
                InvoiceDetailQuantity=itm.InvoiceDetailQuantity,
                ItemInventoryDescription=itm.ItemInventoryDescription,
                ItemInventoryUnitCost=itm.ItemInventoryUnitCost               
            };
            return it;
        }
        public InvoiceModel GetInvoiceDetails(int InvoiceId)
        {           
            var model = _context.tblInvoices.SingleOrDefault(p => p.InvoiceID == InvoiceId);
            var im = MapInvoice(model);
            return im;
        }

        private InvoiceModel MapInvoice( tblInvoice model)
        {
            InvoiceModel im = new InvoiceModel();
            im.InvoiceID = model.InvoiceID;
            im.ClientTypeID = model.ClientTypeID;            
            im.InvoiceJobNumber = model.InvoiceJobNumber;
            im.InvoiceNotes = model.InvoiceNotes;
            im.InvoiceTerms = model.InvoiceTerms;
            im.InvoiceTotal = model.InvoiceTotal;
            im.InvoiceOrderedBy = model.InvoiceOrderedBy;
            im.InvoiceStatus = model.InvoiceStatus;
            im.InvoiceDate = model.InvoiceDate;
            im.InvoiceNumber = model.InvoiceNumber;
            im.Employee = model.EmployeeID.ToString();
            im.CreatedDate = model.CreatedDate;
            im.CreatedUser = model.CreatedUser;
            im.InvoicePONumber = model.InvoicePONumber;          
            im.InvoiceDetails = model.tblInvoiceDetails.Select(p => new InvoiceDetailModel
            {
                InvoiceDetailID = p.InvoiceDetailID,
                ItemInventoryID = p.ItemInventoryID,
                ItemInventoryDescription = p.ItemInventoryDescription,
                ItemInventoryUnitCost = p.ItemInventoryUnitCost,
                NonInventoryItem=p.NonInventoryItem,
                InvoiceDetailQuantity = p.InvoiceDetailQuantity,
                InvoiceDetailLineItemTotal = p.InvoiceDetailLineItemTotal,
                InvoiceDetailCostCodeGL = p.InvoiceDetailCostCodeGL

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
            In.EmployeeID = model.ClientTypeID== 1 ?Convert.ToInt32(model.Employee) : (int?)null;
            In.InvoicePONumber = model.InvoicePONumber;
            In.LocationID = Convert.ToInt16(model.Location);
            In.InvoiceStatus = model.InvoiceStatus == "D" ? "Drafted" : "Completed";
            In.InvoiceTerms = model.InvoiceTerms;
            In.ExternalClientID = model.ExternalClientID;
            In.InvoiceTotal = model.InvoiceTotal;
            In.InvoiceDate = model.InvoiceDate;           
            In.InvoiceNotes = model.InvoiceNotes;
            In.InvoiceOrderedBy = model.InvoiceOrderedBy;
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
                        NonInventoryItem=item.NonInventoryItem,
                        ItemInventoryDescription = item.ItemInventoryDescription,
                        ItemInventoryUnitCost = item.ItemInventoryUnitCost,
                        InvoiceDetailCostCodeGL = item.InvoiceDetailCostCodeGL,
                        InvoiceDetailLineItemTotal = item.InvoiceDetailLineItemTotal,
                        InvoiceDetailQuantity = item.InvoiceDetailQuantity,
                        InvoiceID = item.InvoiceID
                    };
                    In.tblInvoiceDetails.Add(idt);
                    //In.InvoiceTotal += item.InvoiceDetailLineItemTotal;
                    if (model.InvoiceStatus != "D")
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
                            return new InvoiceResult { InvoiceId=-1, Inventory= item.ItemInventoryDescription, Status=false };
                        }
                    }
                }              

            }            
                _context.tblInvoices.Add(In);
                var r = _context.SaveChanges();
            return new InvoiceResult { InvoiceId = In.InvoiceID, Inventory = "", Status = true };            
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
                NonInventoryItem=item.NonInventoryItem,
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
        public int SaveEditItmProd(ItemInventoryModel model)
        {
            tblItemInventory it;
            if (model.ItemInventoryID > 0)
            {
                 it = _context.tblItemInventories.Find(model.ItemInventoryID);
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
            it.ItemInventoryReplacementCost = model.ItemInventoryReplacementCost;
            it.ItemInventoryNumber = model.ItemInventoryNumber;
            it.ItemInventoryDescription = model.ItemInventoryDescription;            
            it.LocationID = model.LocationID;
            var r = _context.SaveChanges();
            return it.ItemInventoryID;
        }
        //savr for edit invoices
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
        public IList<InvoiceModel> GetInvoiceList()
        {
            var Invlst = _context.tblInvoices.Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                InvoiceJobNumber = p.InvoiceJobNumber,
                InvoicePONumber = p.InvoicePONumber,
                InvoiceTotal = p.InvoiceTotal,
                InvoiceTerms = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceStatus = p.InvoiceStatus,
                InvoiceOrderedBy=p.InvoiceOrderedBy,
                CreatedDate = DateTime.Now,
                CreatedUser=p.CreatedUser                
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
            //var lst = from p into _context.tblInvoices
                var grouped = (from p in _context.tblInvoices
                              group p by new { month = p.CreatedDate.Month, year = p.CreatedDate.Year } into d
                              select new InvoicegData { Month = d.Key.month , Year = d.Key.year, Completed = d.Where(p=>p.InvoiceStatus=="Completed").Sum(k=> k.InvoiceTotal)??0, Drafted = d.Where(p => p.InvoiceStatus == "Drafted").Sum(k => k.InvoiceTotal)??0 }).ToList();
            return grouped;
        }
        public IList<InvoiceModel> GetInvoiceLists()
        {
            /*OrderBy(m=>m.InvoiceDate).Take(dt.AddDays()).*/
            int days = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var dt = DateTime.Now.AddDays(-days);
            var Invlst = _context.tblInvoices.Select(p => new InvoiceModel
            {
                InvoiceID = p.InvoiceID,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceDate = p.InvoiceDate,
                ClientTypeID = p.ClientTypeID,
                InvoiceJobNumber = p.InvoiceJobNumber,
                InvoicePONumber = p.InvoicePONumber,
                InvoiceTotal = p.InvoiceTotal,
                InvoiceTerms = p.InvoiceTerms,
                InvoiceNotes = p.InvoiceNotes,
                InvoiceOrderedBy=p.InvoiceOrderedBy,
                InvoiceStatus = p.InvoiceStatus,
                CreatedDate = DateTime.Now,
                CreatedUser = p.CreatedUser
            }).Where(x=>x.InvoiceDate> dt).ToList();
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
                FirstName=p.FirstName,
                LastName=p.LastName,
                EmpCode=p.EmpCode,
                Active=p.Active

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
            return  it;
        }
        public List<JobModel> GetJobDetails()
        {
            var jlst = _context.tblJobs.Select(p => new JobModel
            {
                Job =p.Job,
                JobDescription= p.Job +" "+p.JobDescription,               
                MailAddress=p.MailAddress,
                MailCity=p.MailCity,
                MailState = p.MailState,
                MailZip =p.MailZip,
                ShipAddress=p.ShipAddress,
                ShipCity=p.ShipCity,
                ShipState=p.ShipState,
                ShipZip=p.ShipZip,               
                Active =p.Active

            }).ToList();
            return jlst.OrderByDescending(p=>p.Job).ToList();
        }
        public JobModel GetJobDetails(string id)
        {
            var jlst = _context.tblJobs.Find(id);
            var lst = new JobModel
            {
                Job = jlst.Job,
                JobDescription = jlst.Job + " " + jlst.JobDescription,
                JCCo=jlst.JCCo.Value,
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
                          LocationID=el.LocationID,
                          UseAllAsDefault=el.UseAllAsDefault
                       });

            var emp = lst.FirstOrDefault();
            if (emp != null && !emp.UseAllAsDefault)
                loc = emp.LocationID ==1  ? "Bessemer" : "Houston";

            return loc;
        }

        public IList<ExternalClientModel> GetExternalClientList()
        {
            var clst = _context.tblExternalClients.Select(p => new ExternalClientModel
            {
                ExternalClientID=p.ExternalClientID,
                FirstName=p.FirstName,
                LastName=p.LastName,
                Email=p.Email,
                Employer=p.Employer,
                City=p.City,
                Address=p.Address,
                State=p.State,
                Zip=p.Zip
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
                ItemInventoryQuantity=p.ItemInventoryQuantity,
                ItemInventoryReorderPoint=p.ItemInventoryReorderPoint,
                ItemInventoryExtendedCost=p.ItemInventoryExtendedCost,
                ItemInventorySalesPrice=p.ItemInventorySalesPrice,
                LocationID=p.LocationID,
                ItemInventoryMarkup=p.ItemInventoryMarkup
            }).ToList();
            return lst;
        }
        

    }
}
