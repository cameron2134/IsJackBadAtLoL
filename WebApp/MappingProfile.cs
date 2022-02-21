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
                .ForMember(dest => dest.GameMode, opt => opt.MapFrom(src => src.MatchType.Type));

            CreateMap<MatchDataComment, MatchDataCommentDTO>()
                .ReverseMap();

            CreateMap<GlobalStatistics, GlobalStatisticsDTO>()
                .ReverseMap();

            CreateMap<Summoner, SummonerDTO>();

            CreateMap<WeeklyFeeder, WeeklyFeederDTO>()
                .ForMember(dest => dest.SummonerNickname, opt => opt.MapFrom(src => src.Summoner.Nickname));
        }
    }
}