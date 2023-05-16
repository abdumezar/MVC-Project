using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace Demo.PL.Mapping_Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
        }
    }
}
