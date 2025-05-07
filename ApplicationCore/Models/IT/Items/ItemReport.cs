using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.ItemReports")]
public class ItemReport : EntityBase, IBaseRecord
{
   public ItemReport() 
   { 
   
   }
   // year: 民國, 例如113
   // month: 月份, 0 代表年度結存報表, 
   public ItemReport(int year, int month)
   {
      Year = year;
      Month = month;
      var date = DateTimeHelpers.GetLastDayOfMonth(year.ROCYearToBC(), month);
      Date = $"{date.Year.ToROCYear()}-{date.Month}-{date.Day}";
   }

   public int Year { get; set; }
   public int Month { get; set; }
   public string? Date { get; set; }
   public string? Ps { get; set; }
   
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public DateTime? GetDate()
   {
      if (string.IsNullOrEmpty(Date)) return null;
      var parts = Date.Split("-");
      int year = parts[0].ToInt().ROCYearToBC();
      return new DateTime(year, parts[1].ToInt(), parts[2].ToInt());
   }

   public virtual ICollection<ItemBalanceSheet> ItemBalanceSheets { get; set; } = new List<ItemBalanceSheet>();
}
