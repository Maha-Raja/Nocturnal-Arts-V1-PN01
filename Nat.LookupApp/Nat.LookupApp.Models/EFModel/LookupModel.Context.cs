﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nat.LookupApp.Models.EFModel
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using TLX.CloudCore.Patterns.Repository.Ef6;
    
    public partial class LookupEntities : ElasticScaleContext
    {		
    private void Initialize(){
    			}
    
        private LookupEntities(string connectionStringName)
            : base(connectionStringName)
        {
    		Initialize();
        }
    
    	private LookupEntities()
            : base()
        {
    		Initialize();
        }
    
    	public static LookupEntities CreateContext(bool isMultitenancyEnabled = false)
    	{
    		if(isMultitenancyEnabled)	
    		{
    			return new LookupEntities();
    		}
    		else
    		{
    			return new LookupEntities(ConfigurationManager.ConnectionStrings["LookupEntities"].ConnectionString);
    		}
    	}		
    
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<NAT_LUS_Configuration> NAT_LUS_Configuration { get; set; }
        public virtual DbSet<NAT_LUS_Lookup> NAT_LUS_Lookup { get; set; }
    }
}