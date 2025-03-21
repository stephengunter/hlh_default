using ApplicationCore.Consts;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Azure.Core;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ITApi.Models;

public class ItemLabels
{
   public string Name => "品名";
   public string Code => "代碼";
   public string Ps => "備註";
}
public class ItemsAdminModel
{
   public ItemsAdminModel(ItemsFetchRequest request)
   {
      Request = request;
   }
   public ItemsFetchRequest Request { get; set; }
   public List<BaseOption<int>> ItemOptions { get; set; } = new List<BaseOption<int>>();
   public ItemLabels Labels => new ItemLabels();
   public ItemTransactionLabels TransactionLabels => new ItemTransactionLabels();
}
public class ItemsIndexModel
{
   public ItemsIndexModel(ICollection<ItemViewModel> items)
   {
      Items = items;
   }
   public ItemReportViewModel? LastClosed { get; set; }
   public ICollection<ItemViewModel> Items { get; set; }
}
public class ItemsFetchRequest
{
   public ItemsFetchRequest(bool active)
   {
      Active = active;
   }
   public bool Active { get; set; }
}
public abstract class BaseItemForm
{
   public string Name { get; set; } = String.Empty;
   public string Code { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public bool Active { get; set; }
}
public class ItemAddForm : BaseItemForm
{

}
public class ItemEditForm : BaseItemForm
{
   public bool CanRemove { get; set; }
}