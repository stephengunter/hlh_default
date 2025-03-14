using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;

public class AppsSpecification : Specification<App>
{
   public AppsSpecification()
   {
      Query.Where(x => !x.Removed);
   }
   public AppsSpecification(int id)
   {
      Query.Where(x => !x.Removed && x.Id == id);
   }
   public AppsSpecification(string clientId)
   {
      Query.Where(x => !x.Removed && x.ClientId == clientId);
   }
   public AppsSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => !item.Removed).Where(item => ids.Contains(item.Id));
   }
}

