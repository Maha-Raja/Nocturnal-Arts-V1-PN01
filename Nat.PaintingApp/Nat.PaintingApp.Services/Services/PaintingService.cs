using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nat.PaintingApp.Services.ServiceModels;
using Nat.PaintingApp.Models.EFModel;
using Nat.Core.Exception;
using Nat.PaintingApp.Models.Repositories;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using Nat.Core.Logger;
using Nat.Core.Logger.Extension;
using TLX.CloudCore.KendoX.UI;
using TLX.CloudCore.KendoX.Extensions;
using Nat.Core.Caching;
using Nat.Core.Storage;
using Newtonsoft.Json;
using System.Data.Entity;
using Nat.Core.ServiceClient;
using Nat.PaintingApp.Services.ViewModels;
using System.Collections.ObjectModel;
using Nat.Core.Lookup;
using Nat.Common.Constants;
using Nat.Core.Authentication;

namespace Nat.PaintingApp.Services
{
    public class PaintingService : BaseService<PaintingModel, NAT_PS_Painting>
    {
        private static PaintingService _service;
        public static PaintingService GetInstance(NatLogger logger)
        {
            //if (_service == null)
            {
                _service = new PaintingService();
            }
            _service.SetLogger(logger);
            return _service;
        }

        private PaintingService() : base()
        {

        }


        public async Task<Object> GetPaintingLov()
        {
            using (logger.BeginServiceScope("Get Painting Lov"))
            {
                try
                {
                    async Task<Object> GetPaintingLovFromDB()
                    {
                        logger.LogInformation("Fetch id and name of Painting");
                        Object paintingLov = await uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingLov();
                        return paintingLov;
                    }
                    return await Caching.GetObjectFromCacheAsync<Object>("Paintinglov", 5, GetPaintingLovFromDB);
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        /// <summary>
        /// This method return list of all Paintings
        /// </summary>
        /// <returns>Collection of Painting service model<returns>
        public IEnumerable<PaintingModel> GetAllPainting()
        {
            using (logger.BeginServiceScope("Get All Painting"))
            {
                try
                {
                    IEnumerable<PaintingModel> data = null;
                    logger.LogInformation("Fetch all Painting from repo");
                    IEnumerable<NAT_PS_Painting> PaintingModel = uow.RepositoryAsync<NAT_PS_Painting>().GetAllPainting();
                    if (PaintingModel != null)
                    {
                        data = new PaintingModel().FromDataModelList(PaintingModel);
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

        /// <summary>
        /// This method returns Painting with a given Id
        /// </summary>
        /// <param name="Id">Id of Painting</param>
        /// <returns>Painting service model</returns>
        public async Task<PaintingModel> GetByIdPainting(int Id)

        {
            try
            {
                PaintingModel data = null;
                NAT_PS_Painting PaintingModel = uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingById(Id);
                if (PaintingModel != null)
                {
                    data = new PaintingModel().FromDataModel(PaintingModel);
                    data.Attachments = new PaintingAttachmentModel().FromDataModelList(PaintingModel.NAT_PS_Painting_Attachment.Where(x => x.Active_Flag == true)).ToList();
                    data.PaintingStatusLKP = PaintingModel.Painting_Status_LKP;
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<List<LookupViewModel>> GetPaintingLookup(string lookupType)
        {
            var statuslookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>(lookupType);
            List<LookupViewModel> lookupModelList = new List<LookupViewModel>();
            lookupModelList = statuslookup.Values.ToList();
            return lookupModelList;
        }

        /// <summary>
        /// This method returns Painting with a given Artist Id
        /// </summary>
        /// <param name="Id">Id of Artist</param>
        /// <returns>List of Painting service model</returns>
        public async Task<IEnumerable<PaintingModel>> GetPaintingByArtistIdAsync(int Id)

        {
            try
            {
                IEnumerable<PaintingModel> data = null;
                IEnumerable<NAT_PS_Painting> PaintingModels = await uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingByArtistId(Id);
                if (PaintingModels != null)
                {
                    data = new PaintingModel().FromDataModelList(PaintingModels);
                    return data;
                }
                throw new Exception("asdd");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method returns Painting based on sort parameter
        /// </summary>
        /// <param name="Id">Id of Artist</param>
        /// <returns>List of Painting service model</returns>
        public async Task<IEnumerable<PaintingModel>> FindPaintingsForEvent(string sort)
        {
            try
            {
                IEnumerable<PaintingModel> data = null;
                if (sort == "lastUsed")
                {
                    var eventListResp = await NatClient.ReadAsync<List<EventViewModel>>(NatClient.Method.GET, NatClient.Service.EventService, "Events/PastDays/60");
                    if (!eventListResp.status.IsSuccessStatusCode) throw new Exception("Error occured while fetching events");

                    var eventList = eventListResp.data;
                    var paintingsByLastUsed = eventList.Select(x => new { x.PaintingId, x.EventStartTime })
                        .GroupBy(x => x.PaintingId)
                        .Select(x => new { PaintingId = x.Key, EventStartTime = x.Max(y => y.EventStartTime) }).ToDictionary(x => x.PaintingId, x => x.EventStartTime);

                    var allActivePaintings = new PaintingModel().FromDataModelList(await uow.RepositoryAsync<NAT_PS_Painting>().GetAllActivePaintings()).ToList();

                    if(allActivePaintings != null && paintingsByLastUsed != null)
                    {
                        foreach(var painting in allActivePaintings)
                        {
                            painting.LastEventDate = paintingsByLastUsed.ContainsKey(painting.PaintingId) ? paintingsByLastUsed[painting.PaintingId] : null;
                        }
                    }

                    data = allActivePaintings.OrderByDescending(x => x.LastEventDate);
                }
                else if (sort == "rating")
                {
                    IEnumerable<NAT_PS_Painting> PaintingModels = await uow.RepositoryAsync<NAT_PS_Painting>().GetSortedPaintingByRating();
                    if (PaintingModels != null)
                    {
                        data = new PaintingModel().FromDataModelList(PaintingModels);
                    }
                }
                else if (sort == "ticketsSold")
                {
                    var ticketsListResp = await NatClient.ReadAsync<List<TicketViewModel>>(NatClient.Method.GET, NatClient.Service.TicketBookingService, "Tickets/PastDays/60");
                    if (!ticketsListResp.status.IsSuccessStatusCode) throw new Exception("Unable to get ticket information");
                    var ticketsSoldDictionary = ticketsListResp.data.GroupBy(x => x.EventPaintingId).Select(g => new { PaintingId = g.Key, TicketCount = g.Count() }).ToDictionary(x => x.PaintingId, x => x.TicketCount);

                    IEnumerable<PaintingModel> paintingList;
                    IEnumerable<NAT_PS_Painting> PaintingModels = uow.RepositoryAsync<NAT_PS_Painting>().GetAllPainting();
                    if (PaintingModels != null)
                    {
                        paintingList = new PaintingModel().FromDataModelList(PaintingModels).ToList();
                        foreach (var painting in paintingList)
                        {
                            painting.TicketSold = ticketsSoldDictionary.ContainsKey(painting.PaintingId) ? ticketsSoldDictionary[painting.PaintingId] : 0;
                        }
                        data = paintingList.OrderByDescending(x => x.TicketSold);
                    }
                }
                else if (sort == "alpha")
                {
                    IEnumerable<NAT_PS_Painting> PaintingModels = uow.RepositoryAsync<NAT_PS_Painting>().GetAllPainting().OrderBy(x => x.Painting_Name);
                    if (PaintingModels != null)
                    {
                        data = new PaintingModel().FromDataModelList(PaintingModels);
                    }
                }
                else if (sort == "created")
                {
                    IEnumerable<NAT_PS_Painting> PaintingModels = uow.RepositoryAsync<NAT_PS_Painting>().GetAllPainting().OrderByDescending(x => x.Created_Date);
                    if (PaintingModels != null)
                    {
                        data = new PaintingModel().FromDataModelList(PaintingModels);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// Create: Method for creation of Painting
        /// </summary>
        /// <param name="servicemodel">Service Painting Model</param>
        /// <returns>Painting ID generated for Painting</returns>
        public string CreatePainting(PaintingModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    servicemodel.ActiveFlag = true;
                    servicemodel.ObjectState = ObjectState.Added;

                    Insert(servicemodel);
                    uow.SaveChanges();
                    return Convert.ToString(Get().PaintingId);
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
        /// Update: Method for Updation of Painting record
        /// </summary>
        /// <param name="servicemodel">Service PaintingModel Object</param>
        /// <returns>Bool (true:if record updated, false:if record not updated)</returns>
        public async Task<PaintingModel> UpdatePainting(PaintingModel servicemodel)
        {
            try
            {
                if (servicemodel.PaintingId != 0 || servicemodel.PaintingId > 0)
                {

                    IEnumerable<NAT_Painting_Kit_Item> rolepriv = await uow.RepositoryAsync<NAT_Painting_Kit_Item>().GetkitPaintingMappingByPaintingID(Convert.ToInt32(servicemodel.PaintingId));

                    IEnumerable<PaintingKitItemMappingModel> oldroleprivmodel = new PaintingKitItemMappingModel().FromDataModelList(rolepriv);
                    if (servicemodel.StatusLkp != null)
                        servicemodel.PaintingStatusLKP = servicemodel.StatusLkp;

                    servicemodel.ObjectState = ObjectState.Modified;
                    foreach (PaintingImageModel paintingModel in servicemodel.NatPsPaintingImage)
                    {
                        paintingModel.ObjectState = ObjectState.Modified;
                    }

                    if (servicemodel.NatPaintingKitItem != null)
                    {
                        foreach (PaintingKitItemMappingModel paintingKit in servicemodel.NatPaintingKitItem)
                        {
                            if (paintingKit.PaintingKitItemId > 0)
                            {
                                PaintingKitItemMappingModel priv = oldroleprivmodel.Where(x => x.PaintingKitItemId == paintingKit.PaintingKitItemId).FirstOrDefault();
                                paintingKit.CreatedBy = priv.CreatedBy;
                                paintingKit.CreatedDate = priv.CreatedDate;
                                paintingKit.LastUpdatedBy = priv.LastUpdatedBy;
                                paintingKit.LastUpdatedDate = priv.LastUpdatedDate;

                                paintingKit.ObjectState = ObjectState.Modified;
                            }

                            else if (paintingKit.PaintingKitItemId == 0)
                            {
                                paintingKit.ObjectState = ObjectState.Added;
                            }
                            else if (paintingKit.PaintingKitItemId < 0)
                            {
                                paintingKit.PaintingKitItemId = paintingKit.PaintingKitItemId * -1;
                                paintingKit.ObjectState = ObjectState.Deleted;
                                //uow.RepositoryAsync<NAT_Painting_Kit_Item>().Delete(paintingKit.ToDataModel(paintingKit));
                            }
                        }
                    }

                    //var newIds = servicemodel.NatPaintingKitItem.Select(x => x.KitItemId).ToArray();

                    //foreach (PaintingKitItemMappingModel aa in oldroleprivmodel)
                    //{
                    //    if (!newIds.Contains(aa.KitItemId))
                    //    {
                    //        aa.ObjectState = ObjectState.Deleted;
                    //        uow.RepositoryAsync<NAT_Painting_Kit_Item>().Delete(aa.ToDataModel(aa));

                    //    }
                    //}

                    if (servicemodel.NatPsPaintingVideo != null)
                    {

                        foreach (PaintingVideoModel vid in servicemodel.NatPsPaintingVideo)
                        {
                            if (vid.PaintingVideoId > 0)
                            {
                                foreach (PaintingSupplyModel supp in vid.NatPsPaintingSupply)
                                {
                                    if (supp.PaintingSupplyId > 0)
                                        supp.ObjectState = ObjectState.Modified;

                                    else if (supp.PaintingSupplyId < 0)
                                    {
                                        supp.PaintingSupplyId = supp.PaintingSupplyId * -1;
                                        supp.ObjectState = ObjectState.Deleted;
                                    }
                                    else if (supp.PaintingSupplyId == 0)
                                    {
                                        supp.ObjectState = ObjectState.Added;
                                    }
                                }
                                vid.ObjectState = ObjectState.Modified;
                            }
                            else if (vid.PaintingVideoId < 0)
                            {
                                foreach (PaintingSupplyModel supp in vid.NatPsPaintingSupply)
                                {
                                    supp.PaintingSupplyId = supp.PaintingSupplyId * -1;
                                    supp.ObjectState = ObjectState.Deleted;

                                }
                                vid.PaintingVideoId *= -1;
                                vid.ObjectState = ObjectState.Deleted;
                            }
                            else if (vid.PaintingVideoId == 0)
                            {
                                foreach (PaintingSupplyModel supp in vid.NatPsPaintingSupply)
                                {
                                    supp.ObjectState = ObjectState.Added;

                                }
                                vid.PaintingId = servicemodel.PaintingId;
                                vid.ObjectState = ObjectState.Added;
                            }
                        }
                    }

                    if (servicemodel.Attachments.Count() > 0)
                    {
                        foreach (PaintingAttachmentModel model in servicemodel.Attachments)
                        {
                            if (model.PaintingAttachmentID < 0)
                            {
                                model.PaintingAttachmentID = model.PaintingAttachmentID * -1;
                                NAT_PS_Painting_Attachment attachmentModel = uow.RepositoryAsync<NAT_PS_Painting_Attachment>().Queryable()
                                                                                                                              .Where(x => x.Painting_Attachment_ID == model.PaintingAttachmentID)
                                                                                                                              .AsNoTracking().FirstOrDefault();
                                attachmentModel.Active_Flag = false;
                                attachmentModel.ObjectState = ObjectState.Modified;
                                uow.RepositoryAsync<NAT_PS_Painting_Attachment>().Update(attachmentModel);
                            }
                            if (model.PaintingAttachmentID == 0)
                            {
                                model.PaintingID = servicemodel.PaintingId;
                                model.ObjectState = ObjectState.Added;
                                model.AttachmentUrl = model.AttachmentUrl;
                                model.AttachmentName = model.AttachmentName;
                                model.AttachmentType = model.AttachmentType;
                                model.FileType = model.FileType;
                                model.ActiveFlag = true;
                                model.ObjectState = ObjectState.Added;
                                uow.RepositoryAsync<NAT_PS_Painting_Attachment>().Insert(new PaintingAttachmentModel().ToDataModel(model));
                            }
                        }
                        uow.SaveChanges();
                    }
                    servicemodel.LastUpdatedDate = System.DateTime.UtcNow;
                    base.Update(servicemodel);
                    int updatedRows = uow.SaveChanges();
                    if (updatedRows == 0)
                    {
                        return servicemodel;
                    }
                    return servicemodel;
                }
                else
                {
                    return servicemodel;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method activates Painting 
        /// </summary>
        /// <param name="Id">Id of Painting</param>
        /// <param name="UserName">Username</param>
        public void ActivatePainting(string Id,string UserName)
        {
            try
            {
                NAT_PS_Painting PaintingEf = uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingById(Convert.ToInt32(Id));
                if (PaintingEf != null)
                {
                    PaintingEf.Last_Updated_By = UserName;
                    uow.RepositoryAsync<NAT_PS_Painting>().SetActiveFlag(true, PaintingEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Painting doesnot exists");
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        /// <summary>
        /// This method deactivates Painting 
        /// </summary>
        /// <param name="Id">Id of Painting</param>
        public void DeactivatePainting(string Id,string Username)
        {
            try
            {
                NAT_PS_Painting PaintingEf = uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingById(Convert.ToInt32(Id));
                if (PaintingEf != null)
                {
                    PaintingEf.Last_Updated_By = Username;
                    uow.RepositoryAsync<NAT_PS_Painting>().SetActiveFlag(false, PaintingEf);
                    uow.SaveChanges();
                }
                else
                    throw new ApplicationException("Painting doesnot exists");
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
                var fileName = Guid.NewGuid() + ".jpeg";
                BlobStorage ts = new BlobStorage();
                var imgname = await ts.InsertBlobStorage("PaintingImagesContainerName", bfile, fileName);
                return Environment.GetEnvironmentVariable("PaintingImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("PaintingImagesContainerName") + "/" + imgname;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<PaintingAttachmentModel> UploadAttachment(byte[] bfile, string fileextension)
        {
            try
            {
                var fileName = Guid.NewGuid() + "." + fileextension;

                BlobStorage ts = new BlobStorage();
                var documentname = await ts.InsertBlobStorage("PaintingAttachmentContainerName", bfile, fileName);

                PaintingAttachmentModel documentrequestmodel = new PaintingAttachmentModel();
                documentrequestmodel.AttachmentName = fileName;
                documentrequestmodel.AttachmentUrl = Environment.GetEnvironmentVariable("PaintingAttachmentContainerBaseUrl") + Environment.GetEnvironmentVariable("PaintingAttachmentContainerName") + "/" + documentname;
                documentrequestmodel.FileType = fileextension.Substring(1);
                return documentrequestmodel;
            }

            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<string> UploadVideo(byte[] bfile)
        {
            try
            {
                var fileName = Guid.NewGuid() + ".mp4";
                BlobStorage ts = new BlobStorage();
                var vidname = await ts.InsertBlobStorage("PaintingVideoTutorialsContainerName", bfile, fileName, "video/mp4");
                return Environment.GetEnvironmentVariable("PaintingImagesContainerBaseUrl") + Environment.GetEnvironmentVariable("PaintingVideoTutorialsContainerName") + "/" + vidname;
            }

            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }

        public async Task<PaintingRequestsModel> SubmitPaintingForApproval(PaintingRequestsModel obj)
        {
            try
            {
                obj.JsonData = JsonConvert.SerializeObject(obj);
                obj.Status = Constants.PaintingStatus["Pending"];
                obj.ToDataModel(obj);
                uow.RepositoryAsync<NAT_PS_Painting_Requests>().Insert(obj.ToDataModel(obj));

                await uow.SaveChangesAsync();
                return obj;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> UpdateStatusAsync(PaintingModel serviceModel)
        {
            try
            {
                if (serviceModel.PaintingId != 0 || serviceModel.PaintingId > 0)
                {
                    serviceModel.ObjectState = ObjectState.Modified;
                    uow.RepositoryAsync<NAT_PS_Painting>().Update(serviceModel.ToDataModel(serviceModel));
                    int updatedRows = await uow.SaveChangesAsync();
                    if (updatedRows == 0)
                    {
                        return false;
                    }

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        public async Task<bool> UpdatePaintingStatusAsync(PaintingRequestsModel servicemodel)
        {
            try
            {
                if (servicemodel.RequestId != 0 || servicemodel.RequestId > 0)
                {
                    servicemodel.ObjectState = ObjectState.Modified;
                    uow.RepositoryAsync<NAT_PS_Painting_Requests>().Update(servicemodel.ToDataModel(servicemodel));
                    //base.Update(servicemodel);
                    int updatedRows = await uow.SaveChangesAsync();
                    if (servicemodel.Status == Constants.PaintingStatus["Approved"])
                    {

                        //Painting Main Fields
                        PaintingModel paintingModel = new PaintingModel();
                        paintingModel.PaintingName = servicemodel.PaintingName;
                        paintingModel.Category = servicemodel.Category;
                        paintingModel.Type = servicemodel.Type;
                        paintingModel.ArtistId = servicemodel.ArtistId;
                        paintingModel.InspiredFrom = servicemodel.InspiredFrom;
                        paintingModel.DifficultyLevel = servicemodel.DifficultyLevel;
                        paintingModel.VideoTutorialUrl = servicemodel.VideoTutorialUrl;
                        paintingModel.Price = servicemodel.Price;
                        paintingModel.Tags = servicemodel.Tags;
                        paintingModel.ActiveFlag = true;
                        paintingModel.PaintingStatusLKP = servicemodel.Status;
                        paintingModel.OwnPaintingConsent = servicemodel.OwnPaintingConsent;
                        paintingModel.SellPaintingConsent = servicemodel.SellPaintingConsent;
                        paintingModel.Width = servicemodel.Width;
                        paintingModel.Length = servicemodel.Length;
                        paintingModel.PartnerPaintingFlag = servicemodel.PartnerPaintingFlag;
                        paintingModel.SpecialEventFlag = servicemodel.SpecialEventFlag;
                        paintingModel.Reason = servicemodel.Reason;
                        paintingModel.PaintingMediumLKP = servicemodel.PaintingMediumLKP;
                        paintingModel.Notes = servicemodel.Notes;
                        paintingModel.ObjectState = ObjectState.Added;

                        //// Painting Images
                        paintingModel.NatPsPaintingImage = new List<PaintingImageModel>();
                        foreach (PaintingImageModel element in servicemodel.NatPsPaintingImage)
                        {
                            PaintingImageModel paintingimgModel = new PaintingImageModel();
                            paintingimgModel.PaintingImageId = 0;
                            paintingimgModel.ImagePath = element.ImagePath;
                            paintingimgModel.OriginalImagePath = element.OriginalImagePath;
                            paintingimgModel.PublicURL = element.PublicURL;
                            paintingimgModel.ActiveFlag = true;
                            paintingimgModel.ObjectState = ObjectState.Added;
                            paintingModel.NatPsPaintingImage.Add(paintingimgModel);
                        }

                        //var paintingDM = paintingModel.ToDataModel(paintingModel);
                        Insert(paintingModel);
                        await uow.SaveChangesAsync();
                    }
                    if (updatedRows == 0)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public async Task<PaintingModel> CreatePaintingasync(PaintingRequestsModel servicemodel)
        {
            try
            {
                PaintingModel paintingModel = new PaintingModel();
                paintingModel.PaintingName = servicemodel.PaintingName;
                paintingModel.Category = servicemodel.Category;
                paintingModel.Type = servicemodel.Type;
                paintingModel.ArtistId = servicemodel.ArtistId;
                paintingModel.InspiredFrom = servicemodel.InspiredFrom;
                paintingModel.DifficultyLevel = servicemodel.DifficultyLevel;
                paintingModel.VideoTutorialUrl = servicemodel.VideoTutorialUrl;
                paintingModel.Price = servicemodel.Price;
                paintingModel.Tags = servicemodel.Tags;
                paintingModel.ActiveFlag = true;
                paintingModel.PaintingStatusLKP = servicemodel.Status;
                paintingModel.OwnPaintingConsent = servicemodel.OwnPaintingConsent;
                paintingModel.SellPaintingConsent = servicemodel.SellPaintingConsent;
                paintingModel.Width = servicemodel.Width;
                paintingModel.Length = servicemodel.Length;
                paintingModel.PartnerPaintingFlag = servicemodel.PartnerPaintingFlag;
                paintingModel.SpecialEventFlag = servicemodel.SpecialEventFlag;
                paintingModel.Reason = servicemodel.Reason;
                paintingModel.Notes = servicemodel.Notes;
                paintingModel.PaintingMediumLKP = servicemodel.PaintingMediumLKP;
                paintingModel.CanvasSize = servicemodel.CanvasSize;
                paintingModel.LastUpdatedBy = servicemodel.LastUpdatedBy;
                paintingModel.ObjectState = ObjectState.Added;
                paintingModel.Orientation = servicemodel.Orientation;
                paintingModel.Partner = servicemodel.Partner;
                paintingModel.Occasion = servicemodel.Occasion;
                paintingModel.PaintingAgeGroupTypeLKPID = servicemodel.PaintingAgeGroupTypeLKPID;
                paintingModel.SingleUse = servicemodel.SingleUse;
                paintingModel.SingleUseEventID = servicemodel.SingleUseEventID;
                paintingModel.CreatedBy = servicemodel.CreatedBy;

                //// Painting Images
                paintingModel.NatPsPaintingImage = new List<PaintingImageModel>();
                foreach (PaintingImageModel element in servicemodel.NatPsPaintingImage)
                {
                    PaintingImageModel paintingimgModel = new PaintingImageModel();
                    paintingimgModel.PaintingImageId = 0;
                    paintingimgModel.ImagePath = element.ImagePath;
                    paintingimgModel.OriginalImagePath = element.OriginalImagePath;
                    paintingimgModel.PublicURL = element.PublicURL;
                    paintingimgModel.ActiveFlag = true;
                    paintingimgModel.ObjectState = ObjectState.Added;
                    paintingModel.NatPsPaintingImage.Add(paintingimgModel);
                }

                paintingModel.NatPsPaintingVideo = new List<PaintingVideoModel>();
                if (servicemodel.NatPsPaintingVideo != null)
                {
                    foreach (PaintingVideoModel element in servicemodel.NatPsPaintingVideo)
                    {
                        PaintingVideoModel paintingvidModel = new PaintingVideoModel();
                        paintingvidModel.PaintingVideoId = 0;
                        paintingvidModel.StartTime = element.StartTime;
                        paintingvidModel.EndTime = element.EndTime;
                        paintingvidModel.Title = element.Title;
                        paintingvidModel.Instructions = element.Instructions;

                        paintingvidModel.NatPsPaintingSupply = new List<PaintingSupplyModel>();
                        foreach (PaintingSupplyModel el in element.NatPsPaintingSupply)
                        {
                            PaintingSupplyModel paintingsupModel = new PaintingSupplyModel();
                            paintingsupModel.PaintingSupplyId = 0;
                            paintingsupModel.ItemName = el.ItemName;
                            paintingsupModel.Quantity = el.Quantity;
                            paintingsupModel.ObjectState = ObjectState.Added;
                            paintingvidModel.NatPsPaintingSupply.Add(paintingsupModel);
                        }


                        paintingvidModel.ObjectState = ObjectState.Added;
                        paintingModel.NatPsPaintingVideo.Add(paintingvidModel);
                    }

                }
                paintingModel.NatPaintingKitItem = new List<PaintingKitItemMappingModel>();
                if (servicemodel.NatPaintingKitItem != null)
                {
                    if (servicemodel.NatPaintingKitItem.Count > 0)
                        foreach (PaintingKitItemMappingModel element in servicemodel.NatPaintingKitItem)
                        {
                            PaintingKitItemMappingModel kitmodel = new PaintingKitItemMappingModel();
                            kitmodel.KitItemId = element.KitItemId;
                            kitmodel.PaintingKitItemId = 0;
                            kitmodel.Quantity = element.Quantity;
                            kitmodel.ObjectState = ObjectState.Added;
                            paintingModel.NatPaintingKitItem.Add(kitmodel);
                        }

                }
                Insert(paintingModel);
                await uow.SaveChangesAsync();
                PaintingModel painting = Get();
                if(servicemodel.Attachments != null)
                {
                    if (servicemodel.Attachments.Count() > 0)
                    {
                        paintingModel.Attachments = new List<PaintingAttachmentModel>();
                        foreach (PaintingAttachmentModel model in servicemodel.Attachments)
                        {
                            PaintingAttachmentModel attachment = new PaintingAttachmentModel();
                            attachment.PaintingAttachmentID = 0;
                            attachment.PaintingID = painting.PaintingId;
                            attachment.ObjectState = ObjectState.Added;
                            attachment.AttachmentUrl = model.AttachmentUrl;
                            attachment.AttachmentName = model.AttachmentName;
                            attachment.AttachmentType = model.AttachmentType;
                            attachment.FileType = model.FileType;
                            attachment.ActiveFlag = true;
                            uow.RepositoryAsync<NAT_PS_Painting_Attachment>().Insert(new PaintingAttachmentModel().ToDataModel(attachment));
                        }
                        await uow.SaveChangesAsync();
                    }
                }
                
                return painting;
            }
            catch (Exception e)
            {
                throw ServiceLayerExceptionHandler.Handle(e, logger);
            }
        }



        public async Task<Boolean> BookPaintingEvent(PaintingEventModel servicemodel)
        {
            try
            {
                if (servicemodel != null)
                {
                    PaintingModel data = null;
                    NAT_PS_Painting paintingModel = uow.RepositoryAsync<NAT_PS_Painting>().GetPaintingById(servicemodel.PaintingId);
                    if (paintingModel != null)
                    {
                        servicemodel.ActiveFlag = true;
                        servicemodel.StatusLKP = "1";
                        servicemodel.ObjectState = ObjectState.Added;
                        NAT_PS_Painting_Event dataModel = new PaintingEventModel().ToDataModel(servicemodel);
                        uow.Repository<NAT_PS_Painting_Event>().Insert(dataModel);
                        uow.SaveChangesAsync();
                        return true;
                        //}
                        //else
                        //{ return false; }
                    }
                    else
                    { return false; }
                }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }


        public NAT_PS_Painting_Event CancelPaintingForEvent(string eventcode, int paintingId)
        {
            try
            {
                if (eventcode != null)
                {


                    NAT_PS_Painting_Event paintingeventModel = uow.RepositoryAsync<NAT_PS_Painting_Event>().GetPaintingEventByEventCodeAsync(eventcode, paintingId);
                    paintingeventModel.Status_LKP = "3";
                    paintingeventModel.ObjectState = ObjectState.Modified;
                    if (paintingeventModel != null)
                    {

                        uow.Repository<NAT_PS_Painting_Event>().Update(paintingeventModel);
                        uow.SaveChangesAsync();
                        return paintingeventModel;
                    }
                    else
                    { return null; }
                }
                else
                { return null; }
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }
        public async Task<DataSourceResult> GetAllPaintingList(DataSourceRequest request, Auth.UserModel UserModel)
        {
            try
            {

                var statuslookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("PAINTING_STATUS");

                IEnumerable<NAT_PS_Painting> paintinglistEF;

                var rolechecker = 0;
                foreach (Auth.RoleModel Roles in UserModel.Roles)
                {
                    if (Roles.RoleName == "Artist Manager")
                    {
                        rolechecker = 1;
                    }
                    if (Roles.RoleName == "Artist")
                    {
                        rolechecker = 2;
                    }
                }

                if (rolechecker == 1)
                {
                    var userlist = await NatClient.ReadAsync<IEnumerable<Auth.UserModel>>(NatClient.Method.GET, NatClient.Service.AuthService, "users/" + UserModel.UserId);
                    var ArtistList = userlist.data.Select(x => x.ReferenceId);
                    if (ArtistList != null)
                    {
                        paintinglistEF = await uow.RepositoryAsync<NAT_PS_Painting>().Queryable().Where(x => ArtistList.Contains(x.Artist_ID))
                    .Include(x => x.NAT_PS_Painting_Image)
                    .Include(x => x.NAT_Painting_Kit_Item)
                    .Include(x => x.NAT_PS_Painting_Rating)
                    .Include(x => x.NAT_PS_Painting_Video)
                    .Include("NAT_PS_Painting_Video.NAT_PS_Painting_Supply").ToListAsync();
                    }
                    else
                    {
                        paintinglistEF = new List<NAT_PS_Painting>();
                    }
                }
                else if (rolechecker == 2)
                {
                    if (UserModel.ReferenceId != null)
                    {
                        paintinglistEF = await uow.RepositoryAsync<NAT_PS_Painting>().Queryable().Where(x => x.Artist_ID == UserModel.ReferenceId)
                    .Include(x => x.NAT_PS_Painting_Image)
                    .Include(x => x.NAT_Painting_Kit_Item)
                    .Include(x => x.NAT_PS_Painting_Rating)
                    .Include(x => x.NAT_PS_Painting_Video)
                    .Include("NAT_PS_Painting_Video.NAT_PS_Painting_Supply").ToListAsync();
                    }
                    else
                    {
                        paintinglistEF = new List<NAT_PS_Painting>();
                    }
                }
                else
                {
                    paintinglistEF = await uow.RepositoryAsync<NAT_PS_Painting>().Queryable()
                .Include(x => x.NAT_PS_Painting_Image)
                .Include(x => x.NAT_Painting_Kit_Item)
                .Include(x => x.NAT_PS_Painting_Rating)
                .Include(x => x.NAT_PS_Painting_Video)
                .Include("NAT_PS_Painting_Video.NAT_PS_Painting_Supply").ToListAsync();
                }
                var paintinglistModel = new PaintingModel().FromDataModelList(paintinglistEF).ToList();
                foreach (var painting in paintinglistModel)
                {
                    if (painting.PaintingStatusLKP != null)
                    {
                        painting.PaintingStatus = statuslookup.Values.Where(x => x.HiddenValue == painting.PaintingStatusLKP).Select(x => x.VisibleValue).FirstOrDefault();
                    }
                }
                var result = paintinglistModel.ToDataSourceResult(request);

                var PaintingeventModel = await uow.RepositoryAsync<NAT_PS_Painting_Event>().GetAllEventCalculatedDetails();
                var paintingevent = JsonConvert.SerializeObject(PaintingeventModel);

                IEnumerable<PaintingEventViewModel> PaintingEventData = JsonConvert.DeserializeObject<IEnumerable<PaintingEventViewModel>>(paintingevent);

                if (PaintingEventData != null)
                {
                    var lookupListLocation = await NatClient.ReadAsync<IEnumerable<ViewModels.LocationViewModel>>(NatClient.Method.GET, NatClient.Service.LocationService, "Location");
                    var locationDictionary = lookupListLocation.data.ToDictionary(item => item.LocationShortCode, item => item);


                    Dictionary<int, PaintingEventViewModel> paintingeventDictionary = PaintingEventData.ToDictionary(item => item.PaintingId, item => item);

                    var data = ((IEnumerable<PaintingModel>)result.Data);

                    data = data.Select((x) =>
                    {

                        x.Eventsheld = paintingeventDictionary.ContainsKey(x.PaintingId) ? paintingeventDictionary[x.PaintingId].Eventsheld : 0;
                        x.LastEventDate = paintingeventDictionary.ContainsKey(x.PaintingId) ? paintingeventDictionary[x.PaintingId].LastEventDate : null;
                        x.LastEventLocation = paintingeventDictionary.ContainsKey(x.PaintingId) ? paintingeventDictionary[x.PaintingId].LastEventLocation : null;

                        return x;
                    }).ToList();

                    foreach (var loc in data)
                    {
                        if (loc.LastEventLocation != null)
                        {
                            loc.LastEventLocation = locationDictionary.ContainsKey(loc.LastEventLocation) ? locationDictionary[loc.LastEventLocation].LocationName : null;
                        }
                    }


                    result.Data = data;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ServiceLayerExceptionHandler.Handle(ex, logger);
            }
        }



        public async Task<DataSourceResult> GetPaintingByType(DataSourceRequest request, string type, int id)
        {
            using (logger.BeginServiceScope("Get All Painting By a Status"))
            {
                try
                {
                    IEnumerable<PaintingRequestsModel> data = null;
                    List<PaintingRequestsModel> data1 = new List<PaintingRequestsModel>();
                    IEnumerable<NAT_PS_Painting_Requests> Painting = await uow.RepositoryAsync<NAT_PS_Painting_Requests>().GetPaintingByStatus(type, id);
                    if (Painting != null)
                    {


                        data = new PaintingRequestsModel().FromDataModelList(Painting);

                        foreach (PaintingRequestsModel aa in data)
                        {

                            PaintingRequestsModel neasdw = JsonConvert.DeserializeObject<PaintingRequestsModel>(aa.JsonData);
                            var resp = await NatClient.ReadAsync<ArtistViewModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + neasdw.ArtistId);
                            if (resp.data != null)
                            {
                                ArtistViewModel artistdata = resp.data;
                                if (artistdata.ArtistFirstName != null && artistdata.ArtistLastName != null)
                                {
                                    aa.ArtistName = artistdata.ArtistFirstName + " " + artistdata.ArtistLastName;
                                }
                                else
                                {
                                    aa.ArtistName = null;
                                }

                                aa.NatPsPaintingImage = new Collection<PaintingImageModel>();
                                aa.NatPsPaintingImage = neasdw.NatPsPaintingImage;
                                data1.Add(aa);

                            }

                        }

                        var result = data1.ToDataSourceResult(request);

                        return result;



                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }


        public async Task<DataSourceResult> GetPreviousPaintingsByArtist(DataSourceRequest request)
        {
            using (logger.BeginServiceScope("Get All previously approved and rejected Paintings of an artist"))
            {
                try
                {
                    IEnumerable<PaintingRequestsModel> data = null;
                    List<PaintingRequestsModel> data1 = new List<PaintingRequestsModel>();
                    IEnumerable<NAT_PS_Painting_Requests> Painting = await uow.RepositoryAsync<NAT_PS_Painting_Requests>().GetPreviousPaintingByArtist(new List<string>() {
                        Constants.PaintingStatus["Approved"],
                        Constants.PaintingStatus["Rejected"]
                    });
                    if (Painting != null)
                    {
                        var typelookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("PAINTING_TYPE");
                        var statuslookup = await LookupClient.ReadAsync<ViewModels.LookupViewModel>("PAINTING_STATUS");

                        data = new PaintingRequestsModel().FromDataModelList(Painting);

                        foreach (PaintingRequestsModel aa in data)
                        {

                            aa.Type = aa.Type != null ? typelookup[aa.Type.ToString()].VisibleValue : null;
                            aa.Status = aa.Status != null ? statuslookup[aa.Status.ToString()].VisibleValue : null;

                            PaintingRequestsModel neasdw = JsonConvert.DeserializeObject<PaintingRequestsModel>(aa.JsonData);
                            var resp = await NatClient.ReadAsync<ArtistViewModel>(NatClient.Method.GET, NatClient.Service.ArtistService, "Artists/" + neasdw.ArtistId);
                            ArtistViewModel artistdata = resp.data;
                            aa.ArtistName = artistdata != null ? artistdata.ArtistFirstName + " " + artistdata.ArtistLastName : "";
                            aa.NatPsPaintingImage = new Collection<PaintingImageModel>();
                            aa.NatPsPaintingImage = neasdw.NatPsPaintingImage;
                            data1.Add(aa);
                        }

                        var result = data1.ToDataSourceResult(request);

                        return result;



                    }
                    throw new ApplicationException("asdd");
                }
                catch (Exception ex)
                {
                    throw ServiceLayerExceptionHandler.Handle(ex, logger);
                }
            }
        }

        public async Task<Boolean> checkForDuplicateName(string code, int id)
        {

            var painting = await uow.RepositoryAsync<NAT_PS_Painting>().CheckPaintingName(code, id);

            return painting != null ? true : false;
        }
    }
}