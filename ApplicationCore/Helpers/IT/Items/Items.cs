using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;

namespace ApplicationCore.Helpers.IT;
public static class ItemHelpers
{
   public static ItemViewModel MapViewModel(this Item entity, IMapper mapper)
   {
      var model = mapper.Map<ItemViewModel>(entity);

      return model;
   }
   public static List<ItemViewModel> MapViewModelList(this IEnumerable<Item> entitie, IMapper mapper)
      => entitie.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Item, ItemViewModel> GetPagedList(this IEnumerable<Item> entitie, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Item, ItemViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Item MapEntity(this ItemViewModel model, IMapper mapper, string currentUserId, Item? entity = null)
   {
      if (entity == null) entity = mapper.Map<ItemViewModel, Item>(model);
      else entity = mapper.Map<ItemViewModel, Item>(model, entity);

      return entity;
   }

   public static BaseOption<int> ToOption(this Item entity)
      => new BaseOption<int>(entity.Id, entity.Name);

   public static IEnumerable<Item> GetOrdered(this IEnumerable<Item> entitie)
     => entitie.OrderBy(item => item.Id);
}
