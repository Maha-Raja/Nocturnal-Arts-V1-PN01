//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.EventApp.Models.EFModel
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EventEntities : ElasticScaleContext
    {		
    private void Initialize(){
    			}
    
        private EventEntities(string connectionStringName)
            : base(connectionStringName)
        {
    		Initialize();
        }
    
    	private EventEntities()
            : base()
        {
    		Initialize();
        }
    
    	public static EventEntities CreateContext(bool isMultitenancyEnabled = false)
    	{
    		if(isMultitenancyEnabled)	
    		{
    			return new EventEntities();
    		}
    		else
    		{
    			return new EventEntities(ConfigurationManager.ConnectionStrings["EventEntities"].ConnectionString);
    		}
    	}		
    
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<NAT_ES_Event> NAT_ES_Event { get; set; }
        public virtual DbSet<NAT_ES_Event_Facility> NAT_ES_Event_Facility { get; set; }
        public virtual DbSet<NAT_ES_Event_Feedback> NAT_ES_Event_Feedback { get; set; }
        public virtual DbSet<NAT_ES_Event_Seat> NAT_ES_Event_Seat { get; set; }
        public virtual DbSet<NAT_ES_Event_Seating_Plan> NAT_ES_Event_Seating_Plan { get; set; }
        public virtual DbSet<NAT_ES_Event_Ticket_Price> NAT_ES_Event_Ticket_Price { get; set; }
        public virtual DbSet<NAT_ES_Event_Wait_List> NAT_ES_Event_Wait_List { get; set; }
        public virtual DbSet<NAT_ES_Event_Image> NAT_ES_Event_Image { get; set; }
        public virtual DbSet<NAT_TICKET_SUMMARY_VW> NAT_TICKET_SUMMARY_VW { get; set; }
        public virtual DbSet<NAT_ES_Event_Iteration> NAT_ES_Event_Iteration { get; set; }
        public virtual DbSet<NAT_BOOKED_TICKET_VW> NAT_BOOKED_TICKET_VW { get; set; }
    
        public virtual ObjectResult<Nullable<long>> GetNextSequenceEventCode()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<long>>("GetNextSequenceEventCode");
        }
    }
}
