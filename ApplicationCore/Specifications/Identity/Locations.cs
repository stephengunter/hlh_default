using Ardalis.Specification;
using ApplicationCore.Models.Identity;

namespace ApplicationCore.Specifications.Identity;

public abstract class BaseLocationSpecification : Specification<Location>
{
   public BaseLocationSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class LocationsSpecification : BaseLocationSpecification
{
   public LocationsSpecification()
   {
      
   }
   public LocationsSpecification(int id)
   {
      Query.Where(x => x.Id == id);
   }
}

