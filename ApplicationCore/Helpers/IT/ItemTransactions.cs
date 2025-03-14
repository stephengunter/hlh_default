using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;

namespace ApplicationCore.Helpers.Identity;


public static class ItemTransactionHelpers
{
   public static ItemTransactionViewModel MapViewModel(this ItemTransaction entity, IMapper mapper)
   {
      var model = mapper.Map<ItemTransactionViewModel>(entity);

      return model;
   }
   public static List<ItemTransactionViewModel> MapViewModelList(this IEnumerable<ItemTransaction> entitie, IMapper mapper)
      => entitie.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<ItemTransaction, ItemTransactionViewModel> GetPagedList(this IEnumerable<ItemTransaction> entitie, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<ItemTransaction, ItemTransactionViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static ItemTransaction MapEntity(this ItemTransactionViewModel model, IMapper mapper, string currentUserId, ItemTransaction? entity = null)
   {
      if (entity == null) entity = mapper.Map<ItemTransactionViewModel, ItemTransaction>(model);
      else entity = mapper.Map<ItemTransactionViewModel, ItemTransaction>(model, entity);

      return entity;
   }

   public static IEnumerable<ItemTransaction> GetOrdered(this IEnumerable<ItemTransaction> entitie)
     => entitie.OrderBy(item => item.Date);
}
