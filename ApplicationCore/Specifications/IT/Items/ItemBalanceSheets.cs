using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications.IT;
public class ItemBalanceSheetSpecification : Specification<ItemBalanceSheet>
{
   public ItemBalanceSheetSpecification(ItemReport report, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!) Query.Include(item);
      }
      Query.Where(x => x.ReportId == report.Id);
   }
   public ItemBalanceSheetSpecification(Item entity, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!) Query.Include(item);
      }
      Query.Where(x => x.ItemId == entity.Id);
   }
}
