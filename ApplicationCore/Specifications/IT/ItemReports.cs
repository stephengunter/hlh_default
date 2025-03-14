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