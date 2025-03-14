using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;

namespace ApplicationCore.Helpers.Identity;
public static class ItemReportHelpers
{
   public static ItemReportViewModel MapViewModel(this ItemReport entity, IMapper mapper)
   {
      var model = mapper.Map<ItemReportViewModel>(entity);

      return model;
   }
   public static List<ItemReportViewModel> MapViewModelList(this IEnumerable<ItemReport> entitie, IMapper mapper)
      => entitie.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<ItemReport, ItemReportViewModel> GetPagedList(this IEnumerable<ItemReport> entitie, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<ItemReport, ItemReportViewModel>(entitie, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static ItemReport MapEntity(this ItemReportViewModel model, IMapper mapper, string currentUserId, ItemReport? entity = null)
   {
      if (entity == null) entity = mapper.Map<ItemReportViewModel, ItemReport>(model);
      else entity = mapper.Map<ItemReportViewModel, ItemReport>(model, entity);

      return entity;
   }

   public static IEnumerable<ItemReport> GetOrdered(this IEnumerable<ItemReport> entitie)
     => entitie.OrderBy(item => item.Id);
}
