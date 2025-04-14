using ApplicationCore.Models.IT;
using Ardalis.Specification;
using System.Linq;

namespace ApplicationCore.Specifications.IT;

public abstract class BaseCategoriesSpecification : Specification<Category>
{
   public BaseCategoriesSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class CategoriesSpecification : BaseCategoriesSpecification
{
   public CategoriesSpecification(string entityType)
   {
      Query.Where(item => item.EntityType == entityType);
   }
   public CategoriesSpecification(ICollection<int> ids)
   {
      Query.Where(item => ids.Contains(item.Id));
   }
}
