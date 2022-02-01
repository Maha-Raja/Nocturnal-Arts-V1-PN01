using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.ArtistApp.Services.ServiceModels.ArtistRequest
{
    public class ArtistRequestTableEntityModel : TableEntity
    {
        public ArtistRequestTableEntityModel(string city, string guid, ArtistRequestModel obj)
        {
            this.PartitionKey = city;
            this.RowKey = guid;
            this.FirstName = obj.FirstName;
            this.LastName = obj.LastName;
            this.ContactNo = obj.ContactNo;
            this.Email = obj.Email;
            this.Address = obj.Address;
            this.AddressTwo = obj.AddressTwo;
            this.Password = obj.Password;
            this.ConfirmPassword = obj.ConfirmPassword;
            this.Country = obj.Country;
            this.City = obj.City;
            this.ZipCode = obj.ZipCode;
            this.ArtistAbout = obj.ArtistAbout;
            this.StudioBusiness = obj.StudioBusiness;
            this.PortfolioUrl = obj.PortfolioUrl;
            this.ProfileImageUrl = obj.ProfileImageUrl;
        }
        public ArtistRequestTableEntityModel() { }

        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String ContactNo { get; set; }
        public String Email { get; set; }
        public String Address { get; set; }
        public String AddressTwo { get; set; }
        public String Password { get; set; }
        public String ConfirmPassword { get; set; }
        public String Country { get; set; }
        public String City { get; set; }
        public String ZipCode { get; set; }
        public String ArtistAbout { get; set; }
        public String StudioBusiness { get; set; }
        public String PortfolioUrl { get; set; }
        public String ProfileImageUrl { get; set; }
        public String CityName { get; set; }
        public String HoursPerWeek { get; set; }
        public Nullable<DateTime> RequestTimestamp
        {
            get
            {
                return this.Timestamp.Date;
            }
        }

        public String ArtistFullName
        {
            get; set;
        }
        public String ArtistJsonData { get; set; }
        public ArtistRequestModel ArtistData { get; set; }
        //custom
        public string RejectionReason { get; set; }

    }
}
