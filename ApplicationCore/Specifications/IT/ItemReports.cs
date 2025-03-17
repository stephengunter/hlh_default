using ApplicationCore.Models.IT;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.IT;
public class ItemReportSpecification : Specification<ItemReport>
{
   public ItemReportSpecification(int year)
   {
      Query.Where(x => x.Year == year);
   }
}
public class ItemReportLastClosedSpecification : Specification<ItemReport>
{
   public ItemReportLastClosedSpecification()
   {
      Query.Where(x => x.Month == 0).OrderByDescending(x => x.Year)
             .Take(1);
   }
}