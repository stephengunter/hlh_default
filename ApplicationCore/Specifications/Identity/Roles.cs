using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;
public class RolesIdSpecification : Specification<Role>
{
   public RolesIdSpecification(ICollection<string> ids)
   {
      Query.Where(x => ids.Contains(x.Id));
   }
}