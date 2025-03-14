using ApplicationCore.Views.Identity;
using ApplicationCore.Models.Identity;
using AutoMapper;
using Infrastructure.Views;

namespace ApplicationCore.Helpers.Identity;

public static class RolesHelpers
{
   public static RoleViewModel MapViewModel(this Role role, IMapper mapper)
      => mapper.Map<RoleViewModel>(role);

   public static List<RoleViewModel> MapViewModelList(this IEnumerable<Role> roles, IMapper mapper)
      => roles.Select(item => MapViewModel(item, mapper)).ToList();
}