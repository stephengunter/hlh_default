using Infrastructure.Entities;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class ReportColumn
{
   public ReportColumn(string key, string title, int width)
   {
      Key = key;
      Title = title;
      Width = width;
   }
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public int Width { get; set; }
   
}
