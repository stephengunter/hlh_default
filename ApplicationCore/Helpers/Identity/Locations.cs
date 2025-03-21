using Infrastructure.Helpers;
using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Consts;

namespace ApplicationCore.Helpers.Identity;
public static class LocationHelpers
{
   public static LocationViewModel MapViewModel(this Location entity, IMapper mapper)
   {
      var model = mapper.Map<LocationViewModel>(entity);
      return model;
   }

   public static List<LocationViewModel> MapViewModelList(this IEnumerable<Location> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Location, LocationViewModel> GetPagedList(this IEnumerable<Location> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Location, LocationViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Location MapEntity(this LocationViewModel model, IMapper mapper, string currentUserId, Location? entity = null)
   {
      if (entity == null) entity = mapper.Map<LocationViewModel, Location>(model);
      else entity = mapper.Map<LocationViewModel, Location>(model, entity);

      entity.SetActive(model.Active);

      return entity;
   }

   public static IEnumerable<Location> GetOrdered(this IEnumerable<Location> entities)
     => entities.OrderByDescending(item => item.Order);
}
