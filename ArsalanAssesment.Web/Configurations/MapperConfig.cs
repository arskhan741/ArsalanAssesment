using ArsalanAssesment.Web.DTOs.SaleDTOs;
using ArsalanAssesment.Web.DTOs.UserDTOs;
using ArsalanAssesment.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace ArsalanAssesment.Web.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Sale, CreateSaleDTO>().ReverseMap();
            CreateMap<Sale, DeleteSaleDTO>().ReverseMap();
            CreateMap<Sale, GetSaleDTO>().ReverseMap();
            CreateMap<Sale, UpdateSaleDTO>().ReverseMap();

            CreateMap<IdentityUser, UserDetailsDTO>().ReverseMap();

        }

    }
}
