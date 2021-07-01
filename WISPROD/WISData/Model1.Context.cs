﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WISData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WISEntities : DbContext
    {
        public WISEntities()
            : base("name=WISEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblItemCategory> tblItemCategories { get; set; }
        public virtual DbSet<tblLocation> tblLocations { get; set; }
        public virtual DbSet<tblItemInventoryAudit> tblItemInventoryAudits { get; set; }
        public virtual DbSet<tblClientType> tblClientTypes { get; set; }
        public virtual DbSet<tblItemInventory> tblItemInventories { get; set; }
        public virtual DbSet<tblExternalClient> tblExternalClients { get; set; }
        public virtual DbSet<tblInvoiceSummary> tblInvoiceSummaries { get; set; }
        public virtual DbSet<tblEmployee> tblEmployees { get; set; }
        public virtual DbSet<tblInvoiceDetail> tblInvoiceDetails { get; set; }
        public virtual DbSet<tblJob> tblJobs { get; set; }
        public virtual DbSet<tblEmployeeLocation> tblEmployeeLocations { get; set; }
        public virtual DbSet<tblDefault> tblDefaults { get; set; }
        public virtual DbSet<tblEstimate> tblEstimates { get; set; }
        public virtual DbSet<tblInvoice> tblInvoices { get; set; }
        public virtual DbSet<tblEstimateDetail> tblEstimateDetails { get; set; }
    
        public virtual ObjectResult<GetArchivedInvoice_Result> GetArchivedInvoice(string status)
        {
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetArchivedInvoice_Result>("GetArchivedInvoice", statusParameter);
        }
    
        public virtual ObjectResult<spGetArchivedInvoice_Result> spGetArchivedInvoice(string status)
        {
            var statusParameter = status != null ?
                new ObjectParameter("Status", status) :
                new ObjectParameter("Status", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spGetArchivedInvoice_Result>("spGetArchivedInvoice", statusParameter);
        }
    }
}