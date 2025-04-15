using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;
using System;
using System.Linq;

namespace ApplicationCore.Specifications.IT;

public abstract class BasePropertySpecification : Specification<Property>
{
   public BasePropertySpecification(ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed);
   }
}
public class PropertySpecification : BasePropertySpecification
{
   public PropertySpecification(Category category, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(x => x.CategoryId == category.Id);
   }
   public PropertySpecification(bool deprecated, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(x => x.Deprecated == deprecated);
   }
}
public class PropertyNumberSpecification : Specification<Property>
{
   public PropertyNumberSpecification(string num)
   {
      Query.Where(x => x.Number == num);
   }
   public PropertyNumberSpecification(string num, bool deprecated, PropertyType type)
   {
      Query.Where(x => x.Deprecated == deprecated && x.PropertyType == type && x.Number == num);
   }
}
