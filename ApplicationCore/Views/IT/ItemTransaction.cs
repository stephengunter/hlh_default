using ApplicationCore.Models.IT;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views.IT;

public class ItemTransactionViewModel : EntityBaseView, IBaseRecordView
{
   public DateTime Date { get; set; }
   public int ItemId { get; set; }
   
   public virtual ItemViewModel? Item { get; set; }

   public int Quantity { get; set; }

   public int? DepartmentId { get; set; }

   public string? UserId { get; set; }
   public string? UserName { get; set; }

   public string DatetText => Date.ToDateString();
   public bool In => Quantity > 0;
   public int InQuantity => In ? Quantity : 0;
   public int OutQuantity => In ? 0 : 0 - Quantity;

   public string? Ps { get; set; }
   public bool Removed { get; set; }
   public DateTime CreatedAt { get; set; }
   public string CreatedBy { get; set; } = String.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public string CreatedAtText => CreatedAt.ToDateTimeString();
   public string LastUpdatedText => LastUpdated.ToDateTimeString();

}
