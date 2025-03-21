using ApplicationCore.Models.IT;
using Ardalis.Specification;

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
}
