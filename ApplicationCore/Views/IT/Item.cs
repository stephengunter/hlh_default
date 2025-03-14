using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;

public class ItemViewModel : EntityBaseView, IBaseRecordView
{
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Code { get; set; } = String.Empty;
   public int Price { get; set; }
   public int Stock { get; set; }
   public int SaveStock { get; set; }
   public string? Supplier { get; set; }
   public string Unit { get; set; } = String.Empty;
   public string? Ps { get; set; }

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();

}
