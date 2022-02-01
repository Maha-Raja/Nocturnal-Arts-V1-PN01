using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using TLX.CloudCore.Patterns.Repository.Ef6;
using TLX.CloudCore.Patterns.Repository.Infrastructure;
using TLX.CloudCore.Patterns.Service;
using Nat.VenueApp.Models.EFModel;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.Script.Serialization;
using Microsoft.WindowsAzure.Storage.Blob;
using TLX.CloudCore.Exception;
using Microsoft.WindowsAzure.Storage.Auth;
using Nat.Core.Logger;

namespace Nat.VenueApp.Services
{
    public class BaseService<TEntity, UEntity> : EntityMapperService<TEntity, UEntity>
        where TEntity : class, IServiceModel<UEntity, TEntity>, IObjectState, new()
        where UEntity : class, IObjectState, new()
    {
        public BaseService(UnitOfWork uow)
            : base(uow.DataContext, uow)
        {
        }
        public VenueEntities entityContext;
        public string userId = (Thread.CurrentPrincipal.Identity as ClaimsIdentity).FindFirst("userId") != null ?
            (Thread.CurrentPrincipal.Identity as ClaimsIdentity).FindFirst("userId").Value : "";
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        
	protected NatLogger logger{ 
	    get { return this._logger; }
	}

	private NatLogger _logger;

        protected void SetLogger(NatLogger logger)
        {
            this._logger = logger;
        }

        public BaseService()
        {
            // entityContext =VenueEntities.CreateContext(true); //TODO Read this flag from configuration
            entityContext = VenueEntities.CreateContext(false);
            base.Initialize(entityContext);
        }

        public static List<T> Except<T, TKey>(List<T> items, List<T> exceptItems, Func<T, TKey> getKey)
        {
            var exceptKeys = exceptItems.Select(getKey);

            return items.Where(i => !exceptKeys.Contains(getKey(i))).ToList();
        }

        public bool UpdateNestedCollection<T, TKey, N, Nkey>(List<T> existing, List<T> input, Func<T, TKey> getKey, Func<T, N> getNestedToUpdate, Func<N, Nkey> getNestedKey, out List<T> added, out List<T> modified, int? min = null, int? max = null)
            where T : IObjectState
            where N : IObjectState
        {
            added = Except(input, existing, getKey);

            var deleted = Except(existing, input, getKey);

            modified = Except(input, added, getKey);

            if (max.HasValue)
            {
                if (added.Count() + existing.Count() - deleted.Count() > max)
                {
                    return false;
                }
            }

            if (min.HasValue)
            {
                if (existing.Count() + added.Count() - deleted.Count() < min)
                {
                    return false;
                }
            }

            foreach (var x in added)
            {
                x.ObjectState = ObjectState.Added;

                if (getNestedToUpdate != null)
                {
                    getNestedToUpdate(x).ObjectState = ObjectState.Added;
                }
            }

            foreach (var x in modified)
            {
                x.ObjectState = ObjectState.Modified;

                if (getNestedToUpdate != null)
                {
                    getNestedToUpdate(x).ObjectState = ObjectState.Modified;
                }
            }

            foreach (var x in deleted)
            {
                var child = this;

                ((dynamic)Activator.CreateInstance(typeof(Repository<>).MakeGenericType(typeof(T).BaseType.GetGenericArguments()[0]), child.uow.DataContext, child.uow)).Delete(getKey(x));

                if (getNestedToUpdate != null)
                {
                    ((dynamic)Activator.CreateInstance(typeof(Repository<>).MakeGenericType(typeof(N).BaseType.GetGenericArguments()[0]), child.uow.DataContext, child.uow)).Delete(getNestedKey(getNestedToUpdate(x)));

                    //probably unnecessary, check and remove
                    typeof(N).GetProperty("ObjectState").SetValue(getNestedToUpdate(x), ObjectState.Deleted);
                }
            }

            return true;
        }


        public bool UpdateNestedCollection<T, TKey>(List<T> existing, List<T> input, Func<T, TKey> getKey, out List<T> added, out List<T> modified, int? min = null, int? max = null)
            where T : IObjectState
        {
            added = Except(input, existing, getKey);

            var deleted = Except(existing, input, getKey);

            modified = Except(input, added, getKey);

            if (max.HasValue)
            {
                if (added.Count() + existing.Count() - deleted.Count() > max)
                {
                    return false;
                }
            }

            if (min.HasValue)
            {
                if (existing.Count() + added.Count() - deleted.Count() < min)
                {
                    return false;
                }
            }

            foreach (var x in added)
            {
                x.ObjectState = ObjectState.Added;
            }

            foreach (var x in modified)
            {
                x.ObjectState = ObjectState.Modified;
            }

            foreach (var x in deleted)
            {
                var child = this;

                ((dynamic)Activator.CreateInstance(typeof(Repository<>).MakeGenericType(typeof(T).BaseType.GetGenericArguments()[0]), child.uow.DataContext, child.uow)).Delete(getKey(x));
            }

            return true;
        }

        //public List<int> GetLocationsByUser()
        //{
        //    //Get Logged In User
        //    TmX_User ef = uow.RepositoryAsync<TmX_User>().GetUserById(userId);

        //    List<int> locationList = new List<int>();
        //    locationList.Add(ef.Location_ID.Value);
        //    GetChildLocationsIdsRecursive(ef.Location_ID.Value, locationList);

        //    return locationList;


        //}

        //public void GetChildLocationsIdsRecursive(int locationId, List<int> locationList)
        //{
           
        //        try
        //        {
        //            IQueryable<int> ids = uow.RepositoryAsync<TmX_Location>().GetChildLocationsIds(locationId);
        //            locationList.AddRange(ids);

        //            foreach (int singleChildLocationId in ids)
        //            {
        //                GetChildLocationsIdsRecursive(singleChildLocationId, locationList);
        //            }

                   


        //        }
        //        catch (Exception ex)
        //        {
        //            ServiceLayerExceptionHandler.Handle(ref ex);
        //            throw;
        //        }
            
        //}

        //public List<int> GetUnitsByUser()
        //{
        //    //Get Logged In User
        //    List<int> unitListToReturn = new List<int>();
        //    List<int> unitList =  uow.RepositoryAsync<TmX_Unit_User>().GetUnitIds(userId).ToList();

        //    foreach (int unitId in unitList)
	       // {
        //        unitListToReturn.Add(unitId);
        //        GetChildUnitIdsRecursive(unitId, unitListToReturn);
	       // }
        //    return unitListToReturn;
        //}
        //public void GetChildUnitIdsRecursive(int unitId, List<int> unitList)
        //{

        //    try
        //    {
        //        IQueryable<int> ids = uow.RepositoryAsync<TmX_Unit>().GetChildUnitIds(unitId);
        //        unitList.AddRange(ids);

        //        foreach (int singleChildUnitId in ids)
        //        {
        //            GetChildUnitIdsRecursive(singleChildUnitId, unitList);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ServiceLayerExceptionHandler.Handle(ref ex);
        //        throw;
        //    }

        //}
    }
}