using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelService.Model
{
    public class BaseDTO<TEFModel, TViewModel> : IViewModel<TEFModel, TViewModel>
    {
        public void FromEFModel(TEFModel efModel, TViewModel viewModel)
        {

            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                cfg.CreateMap<TEFModel, TViewModel>();
            });
            IMapper iMapper = config.CreateMapper();
            iMapper.Map(efModel, viewModel);
        }

        public TEFModel ToEFModel(TViewModel viewModel)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.SourceMemberNamingConvention = new PascalCaseNamingConvention();
                cfg.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.CreateMap<TViewModel, TEFModel>();
            });
            IMapper iMapper = config.CreateMapper();
            TEFModel Venue = iMapper.Map<TViewModel, TEFModel>(viewModel);
            return Venue;
        }

        public IEnumerable<TViewModel> FromEFModelList(IEnumerable<TEFModel> efModelList)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                cfg.CreateMap<TEFModel, TViewModel>();
            });
            IMapper iMapper = config.CreateMapper();
            return iMapper.Map<IEnumerable<TViewModel>>(efModelList);
        }

        public IEnumerable<TEFModel> ToEFModelList(IEnumerable<TViewModel> viewModelList)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {
                cfg.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
                cfg.CreateMap<TViewModel, TEFModel>();
            });
            IMapper iMapper = config.CreateMapper();
            return iMapper.Map<IEnumerable<TEFModel>>(viewModelList);
        }
    }
}
