using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications.IT;

public class ItemTransactionsMinYearSpecification : Specification<ItemTransaction, int?>
{
   public ItemTransactionsMinYearSpecification()
   {
      Query.Select(b => (int?)b.Date.Year)
           .OrderBy(b => b.Date.Year)
           .Take(1);
   }
}

public class ItemTransactionSpecification : Specification<ItemTransaction>
{
   public ItemTransactionSpecification(ICollection<string>? includes = null)
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
   public ItemTransactionSpecification(Item entity, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed && item.ItemId == entity.Id);
   }
   public ItemTransactionSpecification(Item entity, DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed && item.ItemId == entity.Id &&
                  item.Date >= sinceDate && item.Date <= endDate);
   }
   public ItemTransactionSpecification(DateTime sinceDate, DateTime endDate, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed && item.Date >= sinceDate && item.Date <= endDate);
   }
}

public class ItemTransactionYearMonthSpecification : Specification<ItemTransaction>
{
   public ItemTransactionYearMonthSpecification(int year, int month, ICollection<string>? includes = null)
   {
      if (includes!.HasItems())
      {
         foreach (var item in includes!)
         {
            Query.Include(item);
         }
      }
      Query.Where(item => !item.Removed && item.Date.Year == year && item.Date.Month == month);
   }
}