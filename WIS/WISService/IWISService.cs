using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WISModels;

namespace WISServiceLayer
{
    public interface IWISService
    {
        IList<ItemInventoryModel> GetInventoryData();
        ItemInventoryModel GetInventoryDataById(int id);
        List<LocationModel> GetLocationList();
        List<ItemCategory> GetItemCategories();
        IList<InvoiceModel> GetInvoiceList();
        IList<InvoiceModel> GetInvoiceLists();
        List<CllientModel> GetClient();
        List<EmployeeModel> GetEmployeeDetails();
        List<JobModel> GetJobDetails();
        InvoiceModel GetInvoiceDetails(int id);
        JobModel GetJobDetails(string id);
        //IList<ItemInventoryModel> GetInventoryzeroQuanty();
    }
}
