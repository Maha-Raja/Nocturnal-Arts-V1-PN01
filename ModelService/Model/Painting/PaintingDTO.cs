using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaintingDataAccessLayer.Model;

namespace ModelService.Model.Painting
{
    public class PaintingDTO
    {
        public int PaintingID { get; set; }
        public Nullable<int> TenantID { get; set; }
        public Nullable<int> ArtistID { get; set; }
        public Nullable<int> CurrencyID { get; set; }
        public Nullable<int> OrientationLKPID { get; set; }
        public Nullable<int> PaintingStatusLKPID { get; set; }
        public string PaintingName { get; set; }
        public Nullable<double> Price { get; set; }
        public string Tags { get; set; }
        public string ApprovedFlag { get; set; }
        public string FeaturedFlag { get; set; }
        public string ActiveFlag { get; set; }
        public Nullable<System.DateTime> EffectiveStartDate { get; set; }
        public Nullable<System.DateTime> EffectiveEndDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }


        public static NAT_PS_Painting toEFModel(PaintingDTO _PaintingDTO)
        {
            MapperConfiguration config = new MapperConfiguration(cfg => {

                cfg.SourceMemberNamingConvention = new PascalCaseNamingConvention();
                cfg.DestinationMemberNamingConvention = new LowerUnderscoreNamingConvention();
                cfg.CreateMap<PaintingDTO, NAT_PS_Painting>();
                //.ForMember(destination => destination.NAT_VS_Venue.Venue_Name, opts => opts.MapFrom(source => source.VenueName)).ReverseMap()
            });
            IMapper iMapper = config.CreateMapper();
            NAT_PS_Painting Painting = iMapper.Map<PaintingDTO, NAT_PS_Painting>(_PaintingDTO);
            return Painting;
        }


        public static PaintingDTO FromEFModel(NAT_PS_Painting _Painting)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<NAT_PS_Painting, PaintingDTO>()
                .ForMember(destination => destination.PaintingName, opts => opts.MapFrom(source => source.Painting_Name)); ;
            });
            IMapper iMapper = config.CreateMapper();
            PaintingDTO _PaintingDTO = iMapper.Map<NAT_PS_Painting, PaintingDTO>(_Painting);
            return _PaintingDTO;
        }

    }
}
