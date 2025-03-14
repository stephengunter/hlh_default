using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;

namespace ApplicationCore.DtoMapper.Identity;
public class RoleMappingProfile : Profile
{
   public RoleMappingProfile()
   {
      CreateMap<Role, RoleViewModel>();

      CreateMap<RoleViewModel, Role>();
   }
}

