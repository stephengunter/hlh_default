using ApplicationCore.Consts;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.Linq;

namespace ITApi.Models;

public class ItemReportsLabels
{
   public string Year => "年度";
   public string Month => "月份";
   public string Ps => "備註";
}
public class ItemReportsIndexModel
{
   public ItemReportsIndexModel(ItemReportsFetchRequest request, ICollection<int> years)
   {
      Request = request;
      YearOptions = years.Select(year => new BaseOption<int>(year, year.ToString())).ToList();     
   }
   public ItemReportsLabels Labels => new ItemReportsLabels();
   public ItemBalanceSheetLabels BalanceSheetLabels => new ItemBalanceSheetLabels();
   public List<BaseOption<int>> YearOptions { get; set; }
   public ItemReportsFetchRequest Request { get; set; }
}
public class ItemReportsFetchRequest
{
   public ItemReportsFetchRequest(int year)
   {
      Year = year;
   }
   public int Year { get; set; }
}

