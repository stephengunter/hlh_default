using Infrastructure.Entities;
using Infrastructure.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views.Identity;

public class LocationViewModel : EntityBaseView
{
   public string Code { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }
}

