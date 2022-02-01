using System;
using System.Collections.Generic;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.CustomerApp.Models.EFModel;
using Nat.Core.ServiceModels;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.CustomerApp.Services.ServiceModels
{
    public class CustomerInquiriesModel : BaseServiceModel<NAT_CS_Customer_Inquiries, CustomerInquiriesModel>, IObjectState
    {


        public Int32 Id { get; set; }

        public string RequestId { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public String CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public bool ActiveFlag { get; set; }
        public ObjectState ObjectState
        {
            get; set;
        }
    }
}
