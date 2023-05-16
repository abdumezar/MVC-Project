using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Demo.PL.Mapping_Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<DepartmentViewModel, Department>().ReverseMap();
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
            CreateMap<RoleViewModel, IdentityRole>()
                .ForMember(D => D.Name, O => O.MapFrom(S => S.RoleName)).ReverseMap();
            CreateMap<EmployeeViewModel, Employee>().ReverseMap();

        }
    }
}
