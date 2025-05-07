using ApplicationCore.Models.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.IT;

public class ItemReportViewModel : EntityBaseView, IBaseRecordView
{
   public int Year { get; set; }
   public int Month { get; set; }
   public string? Date { get; set; }
   public string? Ps { get; set; }

   public bool CanDelete { get; set; }

   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();

   public virtual ICollection<ItemBalanceSheetViewModel> ItemBalanceSheets { get; set; } = new List<ItemBalanceSheetViewModel>();

}
