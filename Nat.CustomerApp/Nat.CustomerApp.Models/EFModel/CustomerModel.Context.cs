//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.CustomerApp.Models.EFModel
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class CustomerEntities : ElasticScaleContext
    {
    private void Initialize(){
    			}
    
        private CustomerEntities(string connectionStringName)
            : base(connectionStringName)
        {
    		Initialize();
        }
    
    	private CustomerEntities()
            : base()
        {
    		Initialize();
        }
    
    	public static CustomerEntities CreateContext(bool isMultitenancyEnabled = false)
    	{
    		if(isMultitenancyEnabled)	
    		{
    			return new CustomerEntities();
    		}
    		else
    		{
    			return new CustomerEntities(ConfigurationManager.ConnectionStrings["CustomerEntities"].ConnectionString);
    		}
    	}
    
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<NAT_CS_Customer> NAT_CS_Customer { get; set; }
        public virtual DbSet<NAT_CS_Customer_Address> NAT_CS_Customer_Address { get; set; }
        public virtual DbSet<NAT_CS_Customer_Event> NAT_CS_Customer_Event { get; set; }
        public virtual DbSet<NAT_CS_Customer_Following> NAT_CS_Customer_Following { get; set; }
        public virtual DbSet<NAT_CS_Customer_Inquiries> NAT_CS_Customer_Inquiries { get; set; }
        public virtual DbSet<NAT_CS_Customer_Liked_Events> NAT_CS_Customer_Liked_Events { get; set; }
    
        public virtual ObjectResult<Nullable<long>> GetNextSequenceCustomerRequest()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<long>>("GetNextSequenceCustomerRequest");
        }
    }
}
