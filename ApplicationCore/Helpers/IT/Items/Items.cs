using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using ApplicationCore.Migrations;

namespace ApplicationCore.Helpers.IT;
public static class ItemHelpers
{
   public static ItemViewModel MapViewModel(this Item entity, IMapper mapper)
   {
      var model = mapper.Map<ItemViewModel>(entity);

      return model;
   }
   public static List<ItemViewModel> MapViewModelList(this IEnumerable<Item> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Item, ItemViewModel> GetPagedList(this IEnumerable<Item> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Item, ItemViewModel>(entities, page, pageSize);
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

   public static IEnumerable<Item> GetOrdered(this IEnumerable<Item> entities)
     => entities.OrderBy(item => item.Id);

   public static ItemBalanceSheet CreateItemBalanceSheet(this Item item, DateTime date,
      int lastStock, IEnumerable<ItemTransaction> transactions)
   {
      var balanceSheet = new ItemBalanceSheet
      {
         ItemId = item.Id,
         LastStock = lastStock,
         Date = date,
      };
      var trans = transactions.Where(x => x.ItemId == item.Id);
      if (trans.HasItems())
      {
         balanceSheet.InQty = trans.Where(x => x.Quantity > 0).Sum(x => x.Quantity);
         balanceSheet.OutQty = Math.Abs(trans.Where(x => x.Quantity < 0).Sum(x => x.Quantity));
      }
      return balanceSheet;
   }

   public static IEnumerable<Item> UpdateStocks(this IEnumerable<Item> entities, IEnumerable<ItemBalanceSheet> sheets,
      IEnumerable<ItemTransaction> transactions)
   {
      foreach (var item in entities)
      {
         int lastStock = 0;
         var sh = sheets.FirstOrDefault(x => x.ItemId == item.Id);
         if (sh != null) lastStock = sh.Stock;

         var trans = transactions.Where(x => x.ItemId == item.Id);
         if (trans.IsNullOrEmpty()) item.Stock = lastStock;
         else item.Stock = lastStock + trans.Sum(x => x.Quantity);
      }
      return entities;
   }
}
