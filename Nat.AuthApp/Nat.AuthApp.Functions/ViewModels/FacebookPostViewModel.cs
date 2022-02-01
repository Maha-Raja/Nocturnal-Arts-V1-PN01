using Nat.AuthApp.Services.ServiceModels;
using Nat.Core.ViewModels;
using Newtonsoft.Json;
using System;
using TLX.CloudCore.Patterns.DataMapper;

namespace Nat.AuthApp.Functions.ViewModels
{
    public class FacebookPostViewModel : BaseAutoViewModel<FacebookPostModel, FacebookPostViewModel>
    {
        public String Message { get; set; }
        public String ImageUrl { get; set; }
    }
}