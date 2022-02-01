using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using TLX.CloudCore.Patterns.Repository.Repositories;
using Nat.PaintingApp.Models.EFModel;
using System;
using System.Threading.Tasks;
using Nat.Common.Constants;

namespace Nat.PaintingApp.Models.Repositories
{
    public static class PaintingRepository
    {


        #region Repository Methods for Painting  



        /// <summary>
        /// Return
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Painting Object</returns>
        public static async Task<Object> GetPaintingLov(this IRepository<NAT_PS_Painting> repository)
        {
            var lov = await repository.Queryable()
                .Select(x => new
                {
                    id = x.Painting_ID,
                    name = x.Painting_Name,
                    rating = x.NAT_PS_Painting_Rating.Select(y => y.Average_Rating_Value).FirstOrDefault(),
                    image = x.NAT_PS_Painting_Image.Select(y => y.Image_Path).FirstOrDefault(),
                    video = x.Video_Tutorial_Url
                }).ToListAsync();
            return lov;
        }


        /// <summary>
        /// This method return list of all Paintings
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>Collection of Painting EF model</returns>
        public static IEnumerable<NAT_PS_Painting> GetAllPainting(this IRepositoryAsync<NAT_PS_Painting> repository)
        {
            return repository.Queryable()
                       .Where(x => x.Active_Flag)
                       .Include(x => x.NAT_PS_Painting_Image).ToList();
        }


        /// <summary>
        /// This method returns Painting with a given Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="PaintingID">Id of Painting</param>
        /// <returns>Painting EF model</returns>
        public static NAT_PS_Painting GetPaintingById(this IRepositoryAsync<NAT_PS_Painting> repository, int PaintingID)
        {
            return repository.Queryable()
                        .Where(x => x.Painting_ID == PaintingID)
                        .Include(x => x.NAT_PS_Painting_Image)
                        .Include(x => x.NAT_PS_Painting_Rating_Log)
                        .Include(x => x.NAT_PS_Painting_Rating)
                        .Include(x => x.NAT_Painting_Kit_Item)
                        .Include(x=> x.NAT_PS_Painting_Attachment)
                        .Include(x => x.NAT_PS_Painting_Video)
                        .Include("NAT_PS_Painting_Video.NAT_PS_Painting_Supply")
                        .AsNoTracking()
                        .FirstOrDefault();
        }


        public static async Task<NAT_PS_Painting> CheckPaintingName(this IRepositoryAsync<NAT_PS_Painting> repository, string name, int id)
        {
            return repository.Queryable()
                       .Where(x => (x.Painting_Name == name && x.Painting_ID != id)).FirstOrDefault();
        }

        /// <summary>
        /// This method returns Painting with a given artist id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of Artist</param>
        /// <returns>List of Painting EF model</returns>
        public static async Task<IEnumerable<NAT_PS_Painting>> GetPaintingByArtistId(this IRepositoryAsync<NAT_PS_Painting> repository, int artistId)
        {
            return await repository.Queryable().Where(x => x.Artist_ID == artistId).Include(x => x.NAT_PS_Painting_Image).ToListAsync();
        }

        /// <summary>
        /// This method returns Painting sorted by rating
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of Artist</param>
        /// <returns>List of Painting EF model</returns>
        public static async Task<IEnumerable<NAT_PS_Painting>> GetSortedPaintingByRating(this IRepositoryAsync<NAT_PS_Painting> repository)
        {
            return await repository.Queryable()
                .Where(x => x.Active_Flag)
                .Include(x => x.NAT_PS_Painting_Rating)
                .Include(x => x.NAT_PS_Painting_Image)
                .OrderByDescending(x => x.NAT_PS_Painting_Rating.Select(y => y.Average_Rating_Value).FirstOrDefault()).ToListAsync();
        }

        /// <summary>
        /// This method returns Painting sorted by rating
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of Artist</param>
        /// <returns>List of Painting EF model</returns>
        public static async Task<IEnumerable<NAT_PS_Painting>> GetAllActivePaintings(this IRepositoryAsync<NAT_PS_Painting> repository)
        {
            return await repository.Queryable()
                .Where(x => x.Active_Flag)
                .Include(x => x.NAT_PS_Painting_Rating)
                .Include(x => x.NAT_PS_Painting_Image).ToListAsync();
        }

        /// <summary>
        /// This method returns Painting in id list
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="artistID">Id of Artist</param>
        /// <returns>List of Painting EF model</returns>
        public static async Task<IEnumerable<NAT_PS_Painting>> GetPaintingsByIds(this IRepositoryAsync<NAT_PS_Painting> repository, List<int> ids)
        {
            return (await repository.Queryable()
                .Include(x => x.NAT_PS_Painting_Rating)
                .Include(x => x.NAT_PS_Painting_Image)
                .Where(x => ids.Contains(x.Painting_ID) && x.Active_Flag)
                .ToListAsync()).OrderBy(x => ids.IndexOf(x.Painting_ID));
        }

        /// <summary>
        /// This method returns calculated data of a painting event
        /// </summary>
        /// <param name="repository"></param>

        public static async Task<IEnumerable<Object>> GetAllEventCalculatedDetails(this IRepositoryAsync<NAT_PS_Painting_Event> repository)
        {
            var paintingeventdata = repository.Queryable().GroupBy(i => i.Painting_ID)
           .Select(g => new
           {

               PaintingId = g.Max(x => x.Painting_ID),

               Eventsheld = (int?)g.Count(x => x.Start_Time < System.DateTime.Now && x.Status_LKP == "1") ?? 0,
               LastEventDate = (DateTime?)g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP == "1").Max(x => x.Start_Time),
               LastEventLocation = g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP == "1").OrderByDescending(x => x.Start_Time).FirstOrDefault().Location,
               //LastEventLocation =( g.Where(x => x.Start_Time < System.DateTime.Now && x.Status_LKP_ID == 1).Max(x => x.Start_Time)).,
               //LastEventLocation = (string)g.OrderByDescending(x => x.Location).Max(x => x.Location),
               //.Where(x => x.Start_Time <= System.DateTime.Now && x.Status_LKP_ID == 1)
           });


            return paintingeventdata;
        }






        /// <summary>
        /// This method returns PaintingEvent with a given reference Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="eventcode">Email Id of Painting</param>
        /// <returnsPaintingEvent EF model</returns>

        public static NAT_PS_Painting_Event GetPaintingEventByEventCodeAsync(this IRepositoryAsync<NAT_PS_Painting_Event> repository, String eventcode, int paintingId)
        {
            return repository.Queryable()
                        .Where(x => x.Reference_ID == eventcode && x.Painting_ID == paintingId).FirstOrDefault();
        }
        public static NAT_PS_Painting_Event UpdateByReferenceId(this IRepositoryAsync<NAT_PS_Painting_Event> repository, int eventId)
        {
            NAT_PS_Painting_Event tempObject= repository.Queryable().Where(x => x.Event_ID == eventId).FirstOrDefault();
            repository.Update(tempObject);
            return tempObject;
        }


        /// <summary>
        /// This method returns person with a given Email Id
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="EmailID">Email Id of Painting</param>
        /// <returns>true/false</returns>
        public static bool GetPersonByEmail(this IRepositoryAsync<NAT_PS_Painting> repository, string EmailID)
        {
            NAT_PS_Painting Person = repository.Queryable().FirstOrDefault();
            if (Person != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method activate/deactivate Painting against provided flag and update in repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="flag">Flag for Activate/Deactivate Painting</param>
        /// <param name="PaintingEf">Painting EF model</param>
        public static void SetActiveFlag(this IRepositoryAsync<NAT_PS_Painting> repository, bool flag, NAT_PS_Painting PaintingEf)
        {
            PaintingEf.Active_Flag = flag;
            repository.Update(PaintingEf, x => x.Active_Flag);
        }
        #endregion

        public static async Task<IEnumerable<NAT_PS_Painting_Requests>> GetPaintingByStatus(this IRepositoryAsync<NAT_PS_Painting_Requests> repository, string status, int id)
        {
            if(id == 0)
            {
                return repository.Queryable().Where(x => x.Status_Lkp == status).ToList();
            }
            else
            {
                return repository.Queryable().Where(x => x.Request_ID==id).ToList();
            }
            
        }

        public static async Task<IEnumerable<NAT_PS_Painting_Requests>> GetPreviousPaintingByArtist(this IRepositoryAsync<NAT_PS_Painting_Requests> repository, List<string> status)
        {
                return repository.Queryable().Where(x => status.Contains(x.Status_Lkp)).ToList();
        }

        public static async Task<IEnumerable<NAT_Painting_Kit_Item>> GetkitPaintingMappingByPaintingID(this IRepositoryAsync<NAT_Painting_Kit_Item> repository, int paintingId)
        {
            return await repository.Queryable().Where(x => x.Painting_ID == paintingId).AsNoTracking().ToListAsync();
        }






    }
}
