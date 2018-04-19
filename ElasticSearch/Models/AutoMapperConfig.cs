using AutoMapper;
using EasySearch.Domain;
using ElasticSearch.Models;

public static class AutoMapperConfig
{
    public static void RegisterMappings()
    {
        Mapper.Initialize(cfg => cfg.CreateMap<PersonData, Person>()
            .ForMember(dest => dest.ssn, src => src.MapFrom(s => s.id))
            .ForMember(dest => dest.id, src => src.Ignore())
            .ForMember(dest => dest.zip, src => src.MapFrom(s => s.postCode))
        );
    }
}