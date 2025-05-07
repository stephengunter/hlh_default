using ApplicationCore.Consts;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ITApi.Models;

public class ItemTransactionLabels
{
   public string Item => "品名";
   public string Quantity => "數量";
   public string Ps => "備註";
   public string DepartmentId => "科室";
   public string UserId => "領用人";
   public string Date => "日期";
   public string Year => "年度";
   public string Month => "月份";
}
public class ItemTransactionsFetchRequest
{
   public ItemTransactionsFetchRequest(int year, int month, int inOut, int? item = null)
   {
      Year = year;
      Month = month;
      InOut = inOut;
      Item = item;
   }
   public int Year { get; set; }
   public int Month { get; set; }
   public int InOut { get; set; }
   public int? Item { get; set; }
}
public class ItemTransactionsIndexModel
{
   public ItemTransactionsIndexModel(ItemTransactionsFetchRequest request)
   {
      Request = request;
   }
   public List<BaseOption<int>> YearOptions { get; set; } = new List<BaseOption<int>>();
   public ItemTransactionsFetchRequest Request { get; set; }
   public List<BaseOption<int>> ItemOptions { get; set; } = new List<BaseOption<int>>();
   public List<BaseOption<int>> InOutOptions { get; set; } = new List<BaseOption<int>>();
   public ItemTransactionLabels Labels => new ItemTransactionLabels();
}

//public class ItemTransactionEditRequest
//{
//   public ItemTransactionEditRequest(BaseItemTransactionForm form)
//   {
//      Form = form;
//   }
//   public ItemTransactionLabels Labels => new ItemTransactionLabels();
//   public BaseItemTransactionForm Form { get; set; }
//}

public abstract class BaseItemTransactionForm
{
   public int ItemId { get; set; }
   public int Quantity { get; set; }
   public bool In { get; set; }
   public int? DepartmentId { get; set; }
   public string? UserId { get; set; }
   public string? Ps { get; set; }
   public string Date { get; set; } = string.Empty;
}
public class ItemTransactionAddForm : BaseItemTransactionForm
{

}
public class ItemTransactionEditForm : BaseItemTransactionForm
{
   public bool CanRemove { get; set; }
}