using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models.Identity;

public class Location : EntityBase, IRemovable, ISortable
{
   public string Code { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public string Name { get; set; } = String.Empty;
   public string? Ps { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

}
