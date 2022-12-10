using AutoMapper;
using MagicVilla_VillaAPI.Dto;
using MagicVilla_VillaAPI.Models;

namespace MagicVilla_VillaAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            // ソースと宛先を定義
            // VillaをもとにVillaDtoをマッピング
            CreateMap<Villa, VillaDto>();
            // VillaDtoをもとにVillaをマッピング（上記の逆マッピング）
            CreateMap<VillaDto, Villa>();

            CreateMap<Villa, VillaUpdateDto>().ReverseMap();
            CreateMap<Villa, VillaCreateDto>().ReverseMap();
        }
    }
}
