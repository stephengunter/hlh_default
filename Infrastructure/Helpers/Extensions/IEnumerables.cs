namespace Infrastructure.Helpers;
public static class IEnumerableHelpers
{
	public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable)
	{
		if (enumerable is null) return true;

		var collection = enumerable as ICollection<T>;
		if (collection != null) return collection.Count < 1;
		return !enumerable.Any();
	}
   
   public static bool HasItems<T>(this IEnumerable<T> enumerable) => !IsNullOrEmpty(enumerable);

   public static async Task ForEachWithIndexAsync<T>(this IEnumerable<T> enumerable, Func<T, int, Task> action)
   {
      if (enumerable == null)
         throw new ArgumentNullException(nameof(enumerable));
      if (action == null)
         throw new ArgumentNullException(nameof(action));

      int index = 0;
      foreach (var item in enumerable)
      {
         await action(item, index);
         index++;
      }
   }

   public static void Each<T>(this IEnumerable<T> ie, Action<T, int> action)
   {
      var i = 0;
      foreach (var e in ie) action(e, i++);
   }

   public static bool AllTheSame(this IEnumerable<int> listA, IEnumerable<int> listB)
		=> listB.All(listA.Contains) && listA.Count() == listB.Count();

   public static bool AllTheSame(this IEnumerable<string> listA, IEnumerable<string> listB)
   {
      // If counts don't match, lists are not the same
      if (listA.Count() != listB.Count()) return false;


      // Use HashSet for efficient lookups and comparison
      var setA = new HashSet<string>(listA);
      var setB = new HashSet<string>(listB);

      // Compare sets for equality
      return setA.SetEquals(setB);
   }

   public static IEnumerable<T> GetList<T>(this IEnumerable<T>? enumerable)
		=> enumerable.IsNullOrEmpty() ? new List<T>() : enumerable!.ToList();

   public static IEnumerable<List<T>> GetInBatches<T>(this IEnumerable<T> source, int batchSize)
   {
      if (source == null) throw new ArgumentNullException(nameof(source));
      if (batchSize <= 0) throw new ArgumentException("Batch size must be greater than zero.", nameof(batchSize));

      List<T> batch = new List<T>(batchSize);
      foreach (var item in source)
      {
         batch.Add(item);
         if (batch.Count == batchSize)
         {
            yield return batch;
            batch = new List<T>(batchSize); // Start a new batch
         }
      }

      // Return the last batch if it contains any items
      if (batch.Count > 0)
         yield return batch;
   }

}
