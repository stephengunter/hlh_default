using ApplicationCore.Consts;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.Identity;
using ApplicationCore.Views.IT;
using Azure.Core;
using Infrastructure.Helpers;
using Infrastructure.Views;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITApi.Models;

public class CategoryLabels
{
   public string EntityType => "EntityType";
   public string Title => "名稱";
   public string Key => "Key";
   public string ParentId => "父分類";
}
public abstract class BaseCategoryForm
{
   public string EntityType { get; set; } = String.Empty;
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;

   public int? ParentId { get; set; }

   public bool Abstract { get; set; }
   public bool Removed { get; set; }
   public int Order { get; set; }
   public bool Active { get; set; }
}
public class CategoryAddForm : BaseCategoryForm
{

}
public class CategoryEditForm : BaseCategoryForm
{
   
}

public class CategoryAddRequest
{
   public CategoryAddRequest(CategoryAddForm form)
   {
      Form = form;
   }
   public CategoryAddForm Form { get; set; }
}
public class CategoryEditRequest
{
   public CategoryEditRequest(CategoryEditForm form, bool canRemove)
   {
      Form = form;
      CanRemove = canRemove;
   }
   public bool CanRemove { get; set; }
   public CategoryEditForm Form { get; set; }
}