using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers.Identity;
public static class ProfilesHelpers
{
   public static ProfilesViewModel MapViewModel(this Profiles profiles, IMapper mapper, Department? department = null)
   { 
      var model = mapper.Map<ProfilesViewModel>(profiles);
      if (department != null) model.Department = department.MapViewModel(mapper);
      return model;
   }

   
   public static Profiles MapEntity(this ProfilesViewModel model, IMapper mapper, string currentUserId, Profiles? entity = null)
   {
      if (entity == null) entity = mapper.Map<ProfilesViewModel, Profiles>(model);
      else entity = mapper.Map<ProfilesViewModel, Profiles>(model, entity);

      if (String.IsNullOrEmpty(model.UserId)) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);
      return entity;
   }

   public static List<ProfilesViewModel> MapViewModelList(this IEnumerable<Profiles> profiles, IMapper mapper)
      => profiles.Select(item => MapViewModel(item, mapper)).ToList();
}
