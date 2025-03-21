using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications.IT;

public abstract class BaseItemSpecification : Specification<Item>
{
   public BaseItemSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class ItemSpecification : BaseItemSpecification
{
}
public class ItembyCodeSpecification : BaseItemSpecification
{
   public ItembyCodeSpecification(string code)
   {
      Query.Where(item => !item.Removed && item.Code == code);
   }
}
