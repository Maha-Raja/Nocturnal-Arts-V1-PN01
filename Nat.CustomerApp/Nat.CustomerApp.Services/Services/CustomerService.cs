using Nat.Common.Constants;
using Nat.Core.Caching;
using Nat.Core.Exception;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using Nat.Core.Notification;
using Nat.Core.Notification.EmailTemplateModels;
using Nat.Core.QueueMessage;
using Nat.Core.ServiceClient;
using Nat.Core.Storage;
using Nat.Core.QueueMessage;
using Nat.Core.Notification.EmailTemplateModels;
using Nat.Core.Notification;
using Nat.Common.Constants;
using Nat.CustomerApp.Services.ViewModels;
using Nat.CustomerApp.Models.EFModel;
using Nat.CustomerApp.Models.Repositories;
using Nat.CustomerApp.Services.ServiceModels;
using Nat.CustomerApp.Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLX.CloudCore.KendoX.Extensions;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.Patterns.Repository.Infrastructure;

namespace Nat.CustomerApp.Services
{
    public class CustomerService : BaseService<CustomerModel, NAT_CS_Customer>
    {
        private static CustomerService _service;
        public static CustomerService GetInstance(NatLogger logger)
        {
            //if (_service == null)
           // {
                _service = new CustomerService();
           // }
            _service.SetLogger(logger);
            return _service;
        }

        private CustomerService() : base()
        {

        }

        /// <summary>
        /// This method return Object having LOV(List Of Values) of customer
        /// </summary>
        /// <returns>Collection of Customer service model<returns>
        public async Task<Object> GetCustomerLov()
        {
            using (logger.BeginServiceScope("Get Customer Lov"))
            {
                try
                {
                    async Task<Object> GetCustomerLovFromDB()
                    {
                        logger.LogInformation("Fetch id and name of Customer");
                        Object customerLov = await uow.RepositoryAsync<NAT_CS_Customer>().GetCustomerLov();
                        if (customerLov != null)
                        {
                            return customerLov;
                        }
                        else
                        {
                            throw new NullReferenceException("Null value returned from DB");
                        }
                    }
                    return await Caching.GetObjectFromCacheAsync("Customerslov", 300, GetCustomerLovFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method return list of all Customers
        /// </summary>
        /// <returns>Collection of Customer service model<returns>
        public IEnumerable<CustomerModel> GetAllCustomer()
        {
            using (logger.BeginServiceScope("Get All Customer"))
            {
                try
                {
                    IEnumerable<CustomerModel> data = null;
                    logger.LogInformation("Fetch all Customer from repo");
                    IEnumerable<NAT_CS_Customer> CustomerModel = uow.RepositoryAsync<NAT_CS_Customer>().GetAllCustomer();
                    if (CustomerModel != null)
                    {
                        data = new CustomerModel().FromDataModelList(CustomerModel);
                        return data;
                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public DataSourceResult GetAllCustomerList(DataSourceRequest request)
        {
            try
            {
                return uow.RepositoryAsync<NAT_CS_Customer>().Queryable().ToDataSourceResult<NAT_CS_Customer, CustomerModel>(request);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Customer with a given Id
        /// </summary>
        /// <param name="Id">Id of Customer</param>
        /// <returns>Customer service model</returns>
        public async Task<CustomerModel> GetByIdCustomer(int Id)

        {
            try
            {
                CustomerModel data = null;
                NAT_CS_Customer CustomerModel = await uow.RepositoryAsync<NAT_CS_Customer>().GetCustomerByIdAsync(Id);
                if (CustomerModel != null)
                {
                    data = new CustomerModel().FromDataModel(CustomerModel);
                    if (data.ResidentialAddressId != null)
                    {
                        var addressGeography = await NatClient.ReadAsync<ViewModels.AddressGeographyViewModel>(NatClient.Method.GET, NatClient.Service.LocationService, "AddressGeography/ParentByType/" + data.NatCsCustomerResidentialAddress.AddressGeographyCode + "/COUNTRY");
                        if (addressGeography != null && addressGeography.status.IsSuccessStatusCode)
                        {
                            data.CountryCode = addressGeography.data.GeographyShortCode;
                        }

                    }
                    
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public Int32 GetFollowersCount(int Id, string options)

        {
            try
            {
                Int32 followers =  uow.RepositoryAsync<NAT_CS_Customer_Following>().GetFollowersCount(Id, options);
                if (followers >= 0)
                {
                    return followers;
                }
                else
                {
                    throw new Exception("Given Option is not Found!");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public Int32 GetEventLikesCount(string eventCode)
        {
            try
            {
                return uow.RepositoryAsync<NAT_CS_Customer_Liked_Events>().GetEventLikesCount(eventCode);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<CustomerBookedEventViewModel>> GetCustomerEvents(int Id)

        {
            try
            {
                var customerEvents = await NatClient.ReadAsync<IEnumerable<CustomerBookedEventViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "GetCustomerBookedEvents/" + Id);
                return customerEvents.data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<IEnumerable<CustomerLikedEventsModel>> GetCustomerLikedEvents(int Id)

        {
            try
            {
                async Task<IEnumerable<CustomerLikedEventsModel>> GetCustomerLikedEventsFromDB()
                {
                    IEnumerable<CustomerLikedEventsModel> data = null;
                    IEnumerable<NAT_CS_Customer_Liked_Events> CustomerLikedEventsModel = await uow.RepositoryAsync<NAT_CS_Customer_Liked_Events>().GetCustomerLikedEventsAsync(Id);
                    if (CustomerLikedEventsModel != null)
                    {
                        data = new CustomerLikedEventsModel().FromDataModelList(CustomerLikedEventsModel);
                        return data;
                    }
                    else
                    {
                        throw new Exception("Unable to fetch list of customer liked events.");
                    }
                }
                return await Caching.GetObjectFromCacheAsync("CustomersLikedEventslov/"+Id, 5, GetCustomerLikedEventsFromDB);
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<IEnumerable<CustomerFollowingModel>> GetCustomerFollowings(int Id, string options)

        {
            try
            {
                async Task<IEnumerable<CustomerFollowingModel>> GetAllVenuesFollowedbyCustomer()
                {
                    IEnumerable<CustomerFollowingModel> data = null;
                    IEnumerable<NAT_CS_Customer_Following> CustomerFollowingModel = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetAllVenuesFollowedbyCustomerAsync(Id);
                    if (CustomerFollowingModel != null)
                    {
                        data = new CustomerFollowingModel().FromDataModelList(CustomerFollowingModel);
                        return data;
                    }
                    throw new Exception("asdd");
                }

                async Task<IEnumerable<CustomerFollowingModel>> GetAllArtistsFollowedbyCustomer()
                {
                    IEnumerable<CustomerFollowingModel> data = null;
                    IEnumerable<NAT_CS_Customer_Following> CustomerFollowingModel = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetAllArtistsFollowedbyCustomerAsync(Id);
                    if (CustomerFollowingModel != null)
                    {
                        data = new CustomerFollowingModel().FromDataModelList(CustomerFollowingModel);
                        return data;
                    }
                    throw new Exception("asdd");
                }

                if(options == Constants.ReferenceType["VENUE"])
                { return await Caching.GetObjectFromCacheAsync<IEnumerable<CustomerFollowingModel>>("CustomerVenueFollowing/" + Id, 0, GetAllVenuesFollowedbyCustomer); }
                else if(options == Constants.ReferenceType["ARTIST"])
                { return await Caching.GetObjectFromCacheAsync<IEnumerable<CustomerFollowingModel>>("CustomerArtistFollowing/" + Id, 0, GetAllArtistsFollowedbyCustomer); }
                throw new Exception("No User Reference Type Found");

                
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of Customer
        /// </summary>
        /// <param name="servicemodel">Service Customer Model</param>
        /// <returns>Customer ID generated for Customer</returns>
        public async Task<string> CreateCustomer(CustomerModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    int rows = uow.SaveChanges();

                    if (rows != 0)
                    {
                        var userModel = new UserModel()
                        {
                            UserId = 0,
                            TenantId = 1,
                            UserName = servicemodel.Email== null ? servicemodel.PhoneNumber : servicemodel.Email,
                            Email = servicemodel.Email,
                            PhoneNumber = servicemodel.PhoneNumber,
                            EmailVerified = servicemodel.EmailVerified,
                            PhoneVerified = servicemodel.PhoneVerified,
                            FirstName = servicemodel.PersonFirstName,
                            LastName = servicemodel.PersonLastName,
                            ActiveFlag = true,
                            Password = servicemodel.Password,
                            RoleCode = "CUSTOMER",
                            ReferenceId = Convert.ToInt64(Get().CustomerId),
                            UserImageURL = servicemodel.PersonProfileImageLink,
                            ReferenceTypeLKP = Constants.UserReferenceType["CUSTOMER"]
                        };

                        var userResp = await NatClient.ReadAsync<string>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Register", requestBody: userModel);

                        if (userResp.status.IsSuccessStatusCode == true)
                        {
                            return Convert.ToString(Get().CustomerId);
                        }
                        else
                        {
                            throw new Exception(userResp.status.message);
                        }
                    }
                    else
                    {
                        throw new Exception("Unable to add user into DB.");
                    }
                }
                else
                {
                    throw new Exception("Invalid user Model.");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of Customer
        /// </summary>
        /// <param name="servicemodel">Service Customer Model</param>
        /// <returns>Customer ID generated for Customer</returns>
        public async Task<TokenModel> SocialMediaCreateCustomer(TokenModel Token)
        {
            try
            {
                if (Token != null)
                {
                    var RegisterUserdata = await NatClient.ReadAsync<TokenModel>(NatClient.Method.POST, NatClient.Service.AuthService, "FacebookLogin", requestBody: Token);
                    if (RegisterUserdata.status.IsSuccessStatusCode && RegisterUserdata.data.NewUserCreated == true)
                    {
                        // NAT_CS_Customer: Place checks for tenant id, Customer_Status_LKP_ID (database level), stripe id  (validations code level) 
                        CustomerModel servicemodel = new CustomerModel();
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;


                        // NAT_CS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                        // Person Email, Date_Of_Birth, Person_Extension (validations code level)

                        servicemodel.PersonFirstName = RegisterUserdata.data.User.FirstName;
                        servicemodel.PersonEmail = RegisterUserdata.data.User.UserName;
                        servicemodel.PersonProfileImageLink = RegisterUserdata.data.User.UserImageURL;
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;
                        

                        Insert(servicemodel);
                        uow.SaveChanges();
                       
                        var customerId = Get().CustomerId;
                        var userReferenceModel = new UserReferenceViewModel()
                        {
                            ReferenceId = customerId,
                            ReferenceTypeLkp = Constants.UserReferenceType["CUSTOMER"],
                            UserId = RegisterUserdata.data.User.UserId
                        };
                        var updateUserRefereceResponse = await NatClient.ReadAsync<UserModel>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Reference", requestBody: userReferenceModel);

                        if (!updateUserRefereceResponse.status.IsSuccessStatusCode)
                        {
                            throw new Exception("An error occured while updating user reference");
                        }

                        RegisterUserdata.data.User.ReferenceId = updateUserRefereceResponse.data.ReferenceId;
                        RegisterUserdata.data.User.ReferenceTypeLKP = updateUserRefereceResponse.data.ReferenceTypeLKP;

                        return RegisterUserdata.data;
                    }
                    else
                    {
                        return RegisterUserdata.data;
                    }
                }
                else
                {
                    throw new Exception("Error occured while authenticating user");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        /// <summary>
        /// Create: Method for creation of Customer
        /// </summary>
        /// <param name="servicemodel">Service Customer Model</param>
        /// <returns>Customer ID generated for Customer</returns>
        public async Task<TokenModel> SocialMediaCreateGoogleCustomer(TokenModel Token)
        {
            try
            {
                if (Token != null)
                {
                    var RegisterUserdata = await NatClient.ReadAsync<TokenModel>(NatClient.Method.POST, NatClient.Service.AuthService, "GoogleLogin", requestBody: Token);
                    if (RegisterUserdata.status.IsSuccessStatusCode && RegisterUserdata.data.NewUserCreated == true)
                    {
                        // NAT_CS_Customer: Place checks for tenant id, Customer_Status_LKP_ID (database level), stripe id  (validations code level) 
                        CustomerModel servicemodel = new CustomerModel();
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;


                        // NAT_CS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                        // Person Email, Date_Of_Birth, Person_Extension (validations code level)
                        servicemodel.PersonFirstName = RegisterUserdata.data.User.FirstName;
                        servicemodel.PersonEmail = RegisterUserdata.data.User.UserName;
                        servicemodel.PersonProfileImageLink = RegisterUserdata.data.User.UserImageURL;
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;


                        Insert(servicemodel);
                        uow.SaveChanges();
                        var customerId = Get().CustomerId;
                        var userReferenceModel = new UserReferenceViewModel()
                        {
                            ReferenceId = customerId,
                            ReferenceTypeLkp = Constants.UserReferenceType["CUSTOMER"],
                            UserId = RegisterUserdata.data.User.UserId
                        };
                        var updateUserRefereceResponse = await NatClient.ReadAsync<UserModel>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Reference", requestBody: userReferenceModel);

                        if (!updateUserRefereceResponse.status.IsSuccessStatusCode)
                        {
                            throw new Exception("An error occured while updating user reference");
                        }

                        RegisterUserdata.data.User.ReferenceId = updateUserRefereceResponse.data.ReferenceId;
                        RegisterUserdata.data.User.ReferenceTypeLKP = updateUserRefereceResponse.data.ReferenceTypeLKP;

                        return RegisterUserdata.data;
                    }
                    else
                    {
                        return RegisterUserdata.data;
                    }
                }
                else
                {
                    throw new Exception("Error occured while authenticating user");
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of Customer
        /// </summary>
        /// <param name="servicemodel">Service Customer Model</param>
        /// <returns>Customer ID generated for Customer</returns>
        public async Task<TokenModel> SocialMediaCreateInstagramCustomer(InstagramModel Code)
        {
            try
            {
                if (Code != null)
                {
                    var RegisterUserdata = await NatClient.ReadAsync<TokenModel>(NatClient.Method.POST, NatClient.Service.AuthService, "InstagramLogin", requestBody: Code);
                    if (RegisterUserdata.status.IsSuccessStatusCode && RegisterUserdata.data.UserFound != false)
                    {
                        // NAT_CS_Customer: Place checks for tenant id, Customer_Status_LKP_ID (database level), stripe id  (validations code level) 
                        CustomerModel servicemodel = new CustomerModel();
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;


                        // NAT_CS_Person: place checks for tenant id,Gender_LKP_ID, Residential address id, shopping address id, billing address id (database level), First_Name, Last_Name, Middle_Name,
                        // Person Email, Date_Of_Birth, Person_Extension (validations code level)
                        servicemodel.PersonFirstName = RegisterUserdata.data.User.FirstName;
                        servicemodel.PersonEmail = RegisterUserdata.data.User.UserName;
                        servicemodel.ActiveFlag = true;
                        servicemodel.ObjectState = ObjectState.Added;


                        Insert(servicemodel);
                        uow.SaveChanges();
                        var customerId = Get().CustomerId;
                        var userReferenceModel = new UserReferenceViewModel()
                        {
                            ReferenceId = customerId,
                            ReferenceTypeLkp = Constants.UserReferenceType["CUSTOMER"],
                            UserId = RegisterUserdata.data.User.UserId
                        };
                        var updateUserRefereceResponse = await NatClient.ReadAsync<object>(NatClient.Method.POST, NatClient.Service.AuthService, "User/Reference", requestBody: userReferenceModel);

                        if (!updateUserRefereceResponse.status.IsSuccessStatusCode)
                        {
                            throw new Exception("An error occured while updating user reference");
                        }
                        return RegisterUserdata.data;
                    }
                    else
                    {
                        return RegisterUserdata.data;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        /// <summary>
        /// Update: Method for Updation of Customer record
        /// </summary>
        /// <param name="servicemodel">Service CustomerModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<CustomerModel> UpdateCustomerAsync(CustomerModel servicemodel)
        {
            try
            {
                if (servicemodel.CustomerId != 0 || servicemodel.CustomerId > 0)
                {
                    // NAT_CS_Customer: stripe id  (validations code level)
                    servicemodel.ObjectState = ObjectState.Modified;

                    if (servicemodel.NatCsCustomerResidentialAddress != null)
                    {
                        if (servicemodel.NatCsCustomerResidentialAddress.AddressId > 0)
                            servicemodel.NatCsCustomerResidentialAddress.ObjectState = ObjectState.Modified;

                        else if (servicemodel.NatCsCustomerResidentialAddress.AddressId < 0)
                        {
                            servicemodel.NatCsCustomerResidentialAddress.AddressId *= -1;
                            servicemodel.NatCsCustomerResidentialAddress.ObjectState = ObjectState.Deleted;
                        }
                        else if (servicemodel.NatCsCustomerResidentialAddress.AddressId == 0)
                        {
                            servicemodel.NatCsCustomerResidentialAddress.ObjectState = ObjectState.Added;
                        }
                    }

                        // NAT_CS_Person: First_Name, Last_Name, Middle_Name, Person Email, Date_Of_Birth, Person_Extension (validations code level)

                    base.Update(servicemodel);
                    int updatedRows = uow.SaveChanges();

                    if (updatedRows == 0)
                    {
                        return null;
                    }
                    else
                    {
                        var userModel = new UserModel()
                        {
                            UserName = servicemodel.PersonEmail,
                            FirstName = servicemodel.PersonFirstName,
                            LastName = servicemodel.PersonLastName,
                            UserImageURL = servicemodel.PersonProfileImageLink,
                            ReferenceId = servicemodel.CustomerId,
                            PhoneNumber = servicemodel.PersonExtension,
                            ReferenceTypeLKP = Constants.UserReferenceType["CUSTOMER"]
                        };

                        var userResp = await NatClient.ReadAsync<string>(NatClient.Method.PUT, NatClient.Service.AuthService, "User/UpdateUserByReference", requestBody: userModel);
                        if (!userResp.status.IsSuccessStatusCode)
                        {
                            throw new Exception(userResp.status.message);
                        }
                        return servicemodel;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method activates Customer 
        /// </summary>
        /// <param name="Id">Id of Customer</param>
        public async Task ActivateCustomer(string Id)
        {
            try
            {
                NAT_CS_Customer CustomerEf = await uow.RepositoryAsync<NAT_CS_Customer>().GetCustomerByIdAsync(Convert.ToInt32(Id));
                if (CustomerEf != null)
                {
                    uow.RepositoryAsync<NAT_CS_Customer>().SetActiveFlag(true, CustomerEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Customer doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates Customer 
        /// </summary>
        /// <param name="Id">Id of Customer</param>
        public async Task DeactivateCustomer(string Id)
        {
            try
            {
                NAT_CS_Customer CustomerEf = await uow.RepositoryAsync<NAT_CS_Customer>().GetCustomerByIdAsync(Convert.ToInt32(Id));
                if (CustomerEf != null)
                {
                    uow.RepositoryAsync<NAT_CS_Customer>().SetActiveFlag(false, CustomerEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Customer doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<string> UploadImage(byte[] bfile)
        {
            try
            {

                //guid generator for img file name
                var fileName = Guid.NewGuid() + ".jpeg";

                //using insertBlobStorage function from Nat.Core
                BlobStorage ts = new BlobStorage();
                var imgname = await ts.InsertBlobStorage("CustomerImagesContainerName", bfile, fileName);

                //returns the name of the img saved in blob
                return Environment.GetEnvironmentVariable("CustomerImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("CustomerImagesContainerName") + "/" + imgname;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Boolean> FollowArtistasync(int CustomerId, int ArtistId, string FollowStatus)
        {
            try
            {
                if (FollowStatus == Constants.FollowStatus["FOLLOW"])
                {
                    CustomerFollowingModel obj = new CustomerFollowingModel();
                    obj.ReferenceId = ArtistId;
                    obj.CustomerId = CustomerId;
                    obj.ActiveFlag = true;
                    obj.ObjectState = ObjectState.Added;
                    obj.CreatedBy = "admin@paintception.com";
                    obj.ReferenceType = Constants.ReferenceType["ARTIST"];
                    //NAT_AS_Artist_Rating_Log rating = new ArtistRatingLogModel().ToDataModel(obj);
                    uow.Repository<NAT_CS_Customer_Following>().Insert(obj.ToDataModel(obj));
                    await uow.SaveChangesAsync();
                    Caching.ClearCache("CustomerArtistFollowing/" + CustomerId);
                    return true;
                }
                else if (FollowStatus == Constants.FollowStatus["UNFOLLOW"])
                {
   
                    NAT_CS_Customer_Following followedartist = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetArtistFollowedEntry(CustomerId, ArtistId);
                    if(followedartist != null)
                    {
                        followedartist.ObjectState = ObjectState.Deleted;
                        await uow.SaveChangesAsync();
                        Caching.ClearCache("CustomerArtistFollowing/" + CustomerId);
                        return true;
                    }
                    else { return false; }

                }
                throw new Exception("FollowStatus not found in the dictionary");
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }



        }

        public async Task<Boolean> FollowVenueasync(string FollowStatus, int CustomerId, int VenueId)
        {
            try
            {
                if (FollowStatus == Constants.FollowStatus["FOLLOW"])
                {
                    CustomerFollowingModel obj = new CustomerFollowingModel();
                    obj.ReferenceId = VenueId;
                    obj.CustomerId = CustomerId;
                    obj.ActiveFlag = true;
                    obj.ObjectState = ObjectState.Added;
                    obj.ReferenceType = Constants.ReferenceType["VENUE"];
                    //NAT_AS_Artist_Rating_Log rating = new ArtistRatingLogModel().ToDataModel(obj);
                    uow.Repository<NAT_CS_Customer_Following>().Insert(obj.ToDataModel(obj));
                    await uow.SaveChangesAsync();
                    Caching.ClearCache("CustomerVenueFollowing/" + CustomerId);
                    return true;
                }
                else if (FollowStatus == Constants.FollowStatus["UNFOLLOW"])
                {

                    NAT_CS_Customer_Following followedvenue = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetVenueFollowedEntry(CustomerId, VenueId);
                    if (followedvenue != null)
                    {
                        followedvenue.ObjectState = ObjectState.Deleted;
                        await uow.SaveChangesAsync();
                        Caching.ClearCache("CustomerVenueFollowing/" + CustomerId);
                        return true;
                    }
                    else { return false; }

                }
                throw new Exception("FollowStatus not found in the dictionary");
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }

        }


        public async Task<DataSourceResult> GetAllArtistFollowers(DataSourceRequest request, int id)
        {

            IEnumerable<NAT_CS_Customer_Following> rat = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetAllFollowersOfArtist(id);
            IEnumerable<CustomerFollowingModel> data1 = new CustomerFollowingModel().FromDataModelList(rat);
            var result = data1.ToDataSourceResult(request);
            return result;

        }

        public async Task<DataSourceResult> GetAllVenueFollowers(DataSourceRequest request, int id)
        {

            IEnumerable<NAT_CS_Customer_Following> rat = await uow.RepositoryAsync<NAT_CS_Customer_Following>().GetAllFollowersOfVenue(id);
            IEnumerable<CustomerFollowingModel> data1 = new CustomerFollowingModel().FromDataModelList(rat);
            var result = data1.ToDataSourceResult(request);
            return result;

        }


        public async Task<CustomerInquiriesModel> CreateCustomerInquiry(CustomerInquiriesModel obj)
        {
            try
            {
                var EventCodeSeq = entityContext.GetNextSequenceCustomerRequest();
                long? nextSequenceValue = EventCodeSeq.Single();
                Int32 eventcode = Convert.ToInt32(nextSequenceValue.Value);


                obj.RequestId = "REQ-" + eventcode;

                obj.ActiveFlag = true;
                obj.ObjectState = ObjectState.Added;
                
                uow.Repository<NAT_CS_Customer_Inquiries>().Insert(obj.ToDataModel(obj));
                await uow.SaveChangesAsync();
                //Send email to the Customer

                NotificationQueueMessage.Receiver r = new NotificationQueueMessage.Receiver();
                r.ReceiverID = obj.EmailAddress;
                r.ReceiverName = obj.Name;

                var dynamicTemplateData = new ContactUsTemplate
                {
                    Name = obj.Name,
                    ReqNo = obj.RequestId,
                    Subject = obj.Subject,
                    Message = obj.Message,

                };

                r.ValueObject = dynamicTemplateData;
                NotificationQueueMessage m = new NotificationQueueMessage("CustomerService", NotificationType.Email, "ContactUsEmail", 0, DateTime.Now, r);
                m.UserId = obj.Id.ToString();
                await new Notification().SendEmail(m);

                //Send email to admin

                NotificationQueueMessage.Receiver r1 = new NotificationQueueMessage.Receiver();
                r1.ReceiverID = Environment.GetEnvironmentVariable("SupportEmail");
                r1.ReceiverName = "Customer Support Group";

                var dynamicTemplateData1 = new ContactUsTemplate
                {
                    Name = obj.Name,
                    ReqNo = obj.RequestId,
                    Subject = obj.Subject,
                    Message = obj.Message,

                };

                r1.ValueObject = dynamicTemplateData1;
                NotificationQueueMessage m1 = new NotificationQueueMessage("CustomerService", NotificationType.Email, "SystemContactUs", 0, DateTime.Now, r1);

                await new Notification().SendEmail(m1);
                return obj;
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
    }
}  