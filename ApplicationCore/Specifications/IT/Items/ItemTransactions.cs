using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications.IT;

public abstract class BaseItemTransactionsSpecification : Specification<ItemTransaction>
{
   public BaseItemTransactionsSpecification(ICollection<string>? includes = null)
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

public class ItemTransactionSpecification : BaseItemTransactionsSpecification
{
   public ItemTransactionSpecification(ICollection<string>? includes = null) : base(includes)
   {
   }
   public ItemTransactionSpecification(Item entity, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(item => item.ItemId == entity.Id);
   }
}
public class ItemTransactionDateSpecification : BaseItemTransactionsSpecification
{
   public ItemTransactionDateSpecification(DateTime sinceDate, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(item => item.Date >= sinceDate);
   }
   public ItemTransactionDateSpecification(DateTime sinceDate, Item entity, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(item => item.Date >= sinceDate && item.ItemId == entity.Id);
   }
   public ItemTransactionDateSpecification(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(item => item.Date >= sinceDate && item.Date <= endDate);
   }
   public ItemTransactionDateSpecification(DateTime sinceDate, DateTime endDate, Item entity, ICollection<string>? includes = null) : base(includes)
   {
      Query.Where(item => item.ItemId == entity.Id &&
                  item.Date >= sinceDate && item.Date <= endDate);
   }
}
