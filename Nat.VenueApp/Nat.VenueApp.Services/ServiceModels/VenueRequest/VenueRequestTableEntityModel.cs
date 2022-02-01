using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.VenueApp.Services.ServiceModels.VenueRequest
{
    public class VenueRequestTableEntityModel : TableEntity
    {

        public VenueRequestTableEntityModel(string city, string guid, VenueRequestModel obj)
        {
            this.PartitionKey = city;
            this.RowKey = guid;
            this.VenueName = obj.VenueName;
            this.Category = obj.Category;
            this.Capacity = obj.Capacity;
            this.Country = obj.Country;
            this.City = obj.City;
            this.Area = obj.Area;
            this.Address = obj.Address;
            this.AddressTwo = obj.AddressTwo;
        }
        public VenueRequestTableEntityModel() { }

        public String VenueName { get; set; }
        public String Category { get; set; }
        public Int32 Capacity { get; set; }
        public String Coordinates { get; set; }
        public String ZipCode { get; set; }
        public String Country { get; set; }
        public String City { get; set; }
        public String Area { get; set; }
        public String Address { get; set; }
        public String AddressTwo { get; set; }
        public String VenueJsonData { get; set; }
        public String Location { get; set; }
        public String HoursPerWeek { get; set; }
        public VenueRequestModel VenueData { get; set; }
        public Nullable<DateTime> CreatedDate { get
            {
                return this.Timestamp.DateTime;
            }
        }

        //custom

        public String RejectionReason { get; set; }

    }
}
