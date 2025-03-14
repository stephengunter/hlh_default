using ApplicationCore.Models.IT;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.IT;
public class ItemSpecification : Specification<Item>
{
   public ItemSpecification()
   {
      Query.Where(item => !item.Removed);
   }
}
public class ItembyCodeSpecification : Specification<Item>
{
   public ItembyCodeSpecification(string code)
   {
      Query.Where(item => !item.Removed && item.Code == code);
   }
}
