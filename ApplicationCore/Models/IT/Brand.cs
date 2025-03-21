using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models.IT;

[Table("IT.Brands")]
public class Brand : EntityBase, IRemovable, ISortable
{
   public string Name { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;

   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);  

}