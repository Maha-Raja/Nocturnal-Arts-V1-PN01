using System;
using System.Collections.Generic;
using System.Linq;
using TLX.CloudCore.Patterns.DataMapper;
using TLX.CloudCore.Patterns.Service;

// should be made abstract
namespace Nat.Core.ServiceModels
{
    public abstract class BaseServiceModel<TEntity, UEntity> : IServiceModel<TEntity, UEntity>
        where TEntity : class,new()
        where UEntity : class,new()
    {
        public BaseServiceModel()
        {
            dynamic child = this;

            try
            {
                child.CreatedBy = "";
              
            }
            catch 
            {
            
            }
        }

        public UEntity FromDataModel(TEntity inputModel)
        {
            if (inputModel == null) { return null; }
            DataMapper.Resolve<TEntity, UEntity>();
            var output = DataMapper.Map<TEntity, UEntity>(inputModel);
            return output;
        }

        public TEntity ToDataModel(UEntity inputModel)
        {
            if (inputModel == null) { return null; }
            DataMapper.Resolve<TEntity, UEntity>();
            var output = DataMapper.Map<UEntity, TEntity>(inputModel);
            return output;
        }

        public IEnumerable<UEntity> FromDataModelList(IEnumerable<TEntity> inputModel)
        {
            if (inputModel == null) { return null; }
            DataMapper.Resolve<TEntity, UEntity>();
            var output = DataMapper.Map<TEntity, UEntity>(inputModel);
            return output;
        }

        public IEnumerable<TEntity> ToDataModelList(IEnumerable<UEntity> inputModel)
        {
            if (inputModel == null) { return null; }
            DataMapper.Resolve<TEntity, UEntity>();
            var output = DataMapper.Map<UEntity, TEntity>(inputModel);
            return output;
        }

        public void SetDefaultStartDate()
        {
            dynamic child = this;

            try
            {
                child.EffectiveStartDate = DateTime.UtcNow;
            }
            catch
            {
                throw new System.Exception("EffectiveStartDate property does not exist");
            }
        }

        public void SetDefaultEndDate()
        {
            dynamic child = this;

            try
            {
                child.EffectiveEndDate = Convert.ToDateTime("1/12/2099");
            }
            catch
            {
                throw new System.Exception("EffectiveEndDate property does not exist");
            }
        }

        public void SetDefaultStartEndDate()
        {
            SetDefaultStartDate();
            SetDefaultEndDate();
        }
    }
}
