using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.Identity;

public class AppViewModel : EntityBaseView, IBaseRecordView
{
   public string Name { get; set; } = String.Empty;
   public string Type { get; set; } = String.Empty;
   public string Url { get; set; } = String.Empty;
   public string Icon { get; set; } = String.Empty;
   public string Roles { get; set; } = String.Empty;
   public string? ClientId { get; set; }
   public string? Encrypt { get; set; }
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

   public string RoleText { get; set; } = string.Empty;



}