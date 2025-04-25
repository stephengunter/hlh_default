using Infrastructure.Helpers;

namespace Infrastructure.Paging;
public class PagedList<T>
{
   public PagedList(IEnumerable<T> list, int pageNumber = 1, int pageSize = -1)
   {
      PageNumber = pageNumber > 0 ? pageNumber : 1;
      PageSize = pageSize > 0 ? pageSize : 999;

      AllItems = list.ToList();
      List = list.GetPaged(PageNumber, PageSize).ToList();
      TotalItems = list.Count();
   }
   public PagedList(IEnumerable<T> list, int pageNumber, int pageSize, bool allItems)
   {
      PageNumber = pageNumber > 0 ? pageNumber : 1;
      PageSize = pageSize > 0 ? pageSize : 999;
      if (allItems) AllItems = list.ToList();
      else AllItems = new List<T>();

      List = list.GetPaged(PageNumber, PageSize).ToList();
      TotalItems = list.Count();
   }
   protected void SetList(List<T> list) => List = list;
   public List<T> AllItems { get; set; }
   public List<T> List { get; set; }
  
   public int TotalItems { get; }
   public int PageNumber { get; } 
   public int PageSize { get; }
   public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

   public bool HasPreviousPage => PageNumber > 1;
   public bool HasNextPage => PageNumber < TotalPages;

   public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;
   public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;
}

public class PagedList<T, V> : PagedList<T>
{
   private List<V> _viewlist = new List<V>();
   public PagedList(IEnumerable<T> list, int pageNumber = 1, int pageSize = -1)
      : base(list, pageNumber, pageSize)
   {

   }
   public PagedList(IEnumerable<T> list, int pageNumber, int pageSize, bool allItems)
      : base(list, pageNumber, pageSize, allItems)
   {

   }

   public List<V> ViewList => _viewlist;
   public void SetViewList(List<V> viewlist)
   {
      _viewlist = viewlist;
      SetList(new List<T>());
   }
}
