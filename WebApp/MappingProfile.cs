using AutoMapper;
using Core;
using Core.DMs;
using Core.DTOs;

namespace WebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MatchData, MatchDataDTO>()
                .ReverseMap();

            CreateMap<MatchDataComment, MatchDataCommentDTO>()
                .ReverseMap();

            CreateMap<GlobalStatistics, GlobalStatisticsDTO>()
                .ReverseMap();
        }
    }
}