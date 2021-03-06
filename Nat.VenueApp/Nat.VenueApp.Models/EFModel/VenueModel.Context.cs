

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Nat.VenueApp.Models.EFModel
{

using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using TLX.CloudCore.Patterns.Repository.Ef6;


public partial class VenueEntities : ElasticScaleContext
{
   private void Initialize(){
		
	}

    private VenueEntities(string connectionStringName)
        : base(connectionStringName)
    {
		Initialize();
    }

	private VenueEntities()
        : base()
    {
		Initialize();
    }

	public static VenueEntities CreateContext(bool isMultitenancyEnabled = false)
	{
		if(isMultitenancyEnabled)	
		{
			return new VenueEntities();
		}
		else
		{
			return new VenueEntities(ConfigurationManager.ConnectionStrings["VenueEntities"].ConnectionString);
		}
	}

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<NAT_VS_Venue> NAT_VS_Venue { get; set; }

    public virtual DbSet<NAT_VS_Venue_Address> NAT_VS_Venue_Address { get; set; }

    public virtual DbSet<NAT_VS_Venue_Event> NAT_VS_Venue_Event { get; set; }

    public virtual DbSet<NAT_VS_Venue_Facility> NAT_VS_Venue_Facility { get; set; }

    public virtual DbSet<NAT_VS_Venue_Hall> NAT_VS_Venue_Hall { get; set; }

    public virtual DbSet<NAT_VS_Venue_Rating> NAT_VS_Venue_Rating { get; set; }

    public virtual DbSet<NAT_VS_Venue_Seat> NAT_VS_Venue_Seat { get; set; }

    public virtual DbSet<NAT_VS_Venue_Seating_Plan> NAT_VS_Venue_Seating_Plan { get; set; }

    public virtual DbSet<NAT_VS_Venue_Image> NAT_VS_Venue_Image { get; set; }

    public virtual DbSet<NAT_VS_Venue_Rating_Log> NAT_VS_Venue_Rating_Log { get; set; }

    public virtual DbSet<NAT_VS_Venue_Contact_Person> NAT_VS_Venue_Contact_Person { get; set; }

    public virtual DbSet<NAT_VS_Venue_Document> NAT_VS_Venue_Document { get; set; }

    public virtual DbSet<NAT_VS_Venue_Artist_Preference> NAT_VS_Venue_Artist_Preference { get; set; }

    public virtual DbSet<NAT_VS_Venue_Bank_Account> NAT_VS_Venue_Bank_Account { get; set; }

    public virtual DbSet<NAT_VS_Venue_Metro_City_Mapping> NAT_VS_Venue_Metro_City_Mapping { get; set; }

    public virtual DbSet<NAT_Venue_VW> NAT_Venue_VW { get; set; }

}

}

