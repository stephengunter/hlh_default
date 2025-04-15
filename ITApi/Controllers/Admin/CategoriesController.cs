using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Services.IT;
using ApplicationCore.Views.IT;
using ApplicationCore.Models.IT;
using ITApi.Models;
using Infrastructure.Helpers;
using ApplicationCore.Migrations;
using ApplicationCore.Helpers.IT;

namespace ITApi.Controllers.Admin;

public class CategoriesController : BaseAdminController
{
   private readonly ICategoryService _categoryService;
   private readonly IMapper _mapper;
   private readonly CategoryLabels _labels = new CategoryLabels();

   public CategoriesController(ICategoryService categoryService, IMapper mapper)
   {
      _categoryService = categoryService;
      _mapper = mapper;
   }

   [HttpGet]
   public async Task<ActionResult<ICollection<CategoryViewModel>>> Index(string type)
   {
      ValidateEntityType(type);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var categories = await _categoryService.FetchAsync(type);
      categories = categories.GetOrdered();

      return categories.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public async Task<ActionResult<CategoryAddRequest>> Create()
   {
      var form = new CategoryAddForm();
      return new CategoryAddRequest(form);
   }

   [HttpPost]
   public async Task<ActionResult<CategoryViewModel>> Store([FromBody] CategoryAddForm model)
   {
      await ValidateRequestAsync(model, 0);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Category();
      model.SetValuesTo(entity);

      entity = await _categoryService.CreateAsync(entity);

      return Ok(entity.MapViewModel(_mapper));
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<CategoryEditRequest>> Edit(int id)
   {
      var entity = await _categoryService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new CategoryEditForm();
      entity.SetValuesTo(form);

      return new CategoryEditRequest(form, false);
   }
   [HttpPut("{id}")]
   public async Task<ActionResult> Update(int id, [FromBody] CategoryEditForm model)
   {
      var entity = await _categoryService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(model, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      model.SetValuesTo(entity);

      await _categoryService.UpdateAsync(entity);

      return NoContent();
   }

   [HttpDelete("{id}")]
   public async Task<IActionResult> Remove(int id)
   {
      var entity = await _categoryService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await _categoryService.RemoveAsync(entity);

      return NoContent();
   }

   async Task ValidateRequestAsync(BaseCategoryForm model, int id)
   {
      if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError(nameof(model.Title), ValidationMessages.Required(_labels.Title));
      ValidateEntityType(model.EntityType);
      if (!ModelState.IsValid) return;

      await ValidateExistAsync(model, id);
   }

   void ValidateEntityType(string type)
   {
      type = type.ToLower();
      if (type == nameof(Property).ToLower() || type == nameof(device).ToLower() || type == nameof(Property).ToLower())
      {
      }
      else
      {
         ModelState.AddModelError("EntityType", ValidationMessages.NotExist(type));
      }
   }

   async Task ValidateExistAsync(BaseCategoryForm model, int id)
   {
      var categories = await _categoryService.FetchAsync(model.EntityType);
      var exist = categories.FirstOrDefault(x => x.Title == model.Title);
      if (exist != null && exist.Id != id)
      {
         ModelState.AddModelError(nameof(model.Title), ValidationMessages.Duplicate(_labels.Title));
      }
   }

}
