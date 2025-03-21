using Ardalis.Specification;
using ApplicationCore.Models.Identity;
using ApplicationCore.Models.IT;

namespace ApplicationCore.Specifications.Identity;

public abstract class BaseAppsSpecification : Specification<App>
{
   public BaseAppsSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class AppsSpecification : BaseAppsSpecification
{
   public AppsSpecification()
   {
   }
   public AppsSpecification(int id)
   {
      Query.Where(x => x.Id == id);
   }
   public AppsSpecification(string clientId)
   {
      Query.Where(x => x.ClientId == clientId);
   }
   public AppsSpecification(IEnumerable<int> ids)
   {
      Query.Where(item => ids.Contains(item.Id));
   }
}

