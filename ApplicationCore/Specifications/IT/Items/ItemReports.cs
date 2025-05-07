using ApplicationCore.Models.IT;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.IT;
public class ItemReportSpecification : Specification<ItemReport>
{
   public ItemReportSpecification(int year)
   {
      Query.Where(x => x.Year == year);
   }
   public ItemReportSpecification(int year, int month)
   {
      Query.Where(x => x.Year == year && x.Month == month);
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
public class ItemReportLatestSpecification : Specification<ItemReport>
{
   public ItemReportLatestSpecification()
   {
      Query.Where(x => x.Month > 0).OrderByDescending(x => x.Year).ThenByDescending(x => x.Year)
             .Take(1);
   }
}