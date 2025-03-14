using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Services.IT;
using ITApi.Models;
using Azure.Core;
using ApplicationCore.Services.Identity;
using System;

namespace ITApi.Controllers.Admin;

public class ItemsController : BaseAdminController
{
   private readonly IItemService _itemService;
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;

   public ItemsController(IItemService itemService, IDepartmentsService departmentsService, IMapper mapper)
   {
      _itemService = itemService; 
      _departmentsService = departmentsService;
      _mapper = mapper;
   }

   [HttpGet("init")]
   public async Task<ActionResult<ItemsIndexModel>> Init()
   {
      var model = new ItemsIndexModel(new ItemsFetchRequest(true));

      var items = await _itemService.FetchAsync();
      items = items.Where(x => x.Active).ToList();
      model.ItemOptions = items.Select(item => item.ToOption()).ToList();

      return model;
   }

   [HttpGet]   
   public async Task<ActionResult<ICollection<ItemViewModel>>> Index(bool active)
   {
      var items = await _itemService.FetchAsync();
      items = items.Where(x => x.Active == active);
      return items.MapViewModelList(_mapper);
   }
   [HttpGet("create")]
   public ActionResult<ItemAddForm> Create()
      => new ItemAddForm();

   [HttpPost]
   public async Task<ActionResult> Store([FromBody] ItemAddForm form)
   {
      await ValidateRequestAsync(form);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var entity = new Item();
      form.SetValuesTo(entity);
      entity.SetActive(form.Active);

      await _itemService.CreateAsync(entity, User.Id());
      return Ok();
   }
   [HttpGet("edit/{id}")]
   public async Task<ActionResult<ItemEditForm>> Edit(int id)
   {
      var entity = await _itemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      var form = new ItemEditForm();
      entity.SetValuesTo(form);
      form.Active = entity.Order > -1;
      return form;
   }
   [HttpPut("{id}")]
   public async Task<IActionResult> Update(int id, [FromBody] ItemEditForm form)
   {
      var entity = await _itemService.GetByIdAsync(id);
      if (entity == null) return NotFound();

      await ValidateRequestAsync(form, id);
      if (!ModelState.IsValid) return BadRequest(ModelState);

      form.SetValuesTo(entity);
      entity.SetActive(form.Active);

      await _itemService.UpdateAsync(entity, User.Id());
      return NoContent();
   }
   //[HttpPut("reset-client-secret/{id}")]
   //public async Task<IActionResult> ResetClientSecret(int id)
   //{
   //   var app = await _appService.GetByIdAsync(id);
   //   if (app == null) return NotFound();

   //   if (!app.Type.EqualTo(AppTypes.Api))
   //   {
   //      ModelState.AddModelError("type", "AppType Not Valid.");
   //      return BadRequest(ModelState);
   //   }

   //   await _appService.ResetClientSecretAsync(app);
   //   return NoContent();
   //}
   //[HttpDelete("{id}")]
   //public async Task<IActionResult> Remove(int id)
   //{
   //   var app = await _appService.GetByIdAsync(id);
   //   if (app == null) return NotFound();

   //   await _appService.RemoveAsync(app, User.Id());

   //   return NoContent();
   //}

   async Task ValidateRequestAsync(BaseItemForm model, int id = 0)
   {
      var labels = new ItemLabels();
      if (string.IsNullOrEmpty(model.Name))
      {
         ModelState.AddModelError(nameof(model.Name), ValidationMessages.Required(labels.Name));
      }
      if (string.IsNullOrEmpty(model.Code))
      {
         ModelState.AddModelError(nameof(model.Code), ValidationMessages.Required(labels.Code));
      }

      if (!ModelState.IsValid) return;
      if (id == 0) return;

      var exist = await _itemService.FindByCodeAsync(model.Code);
      if (exist is not null && exist.Id != id)
      {
         ModelState.AddModelError(nameof(model.Code), ValidationMessages.Duplicate(labels.Code));
      }
   }

   //void ValidateType(string type)
   //{
   //   if (string.IsNullOrEmpty(type))
   //   {
   //      ModelState.AddModelError("type", ValidationMessages.Required(type));
   //   }

   //   if (type.EqualTo(AppTypes.Spa) || type.EqualTo(AppTypes.Api)) return;
   //   ModelState.AddModelError("type", ValidationMessages.NotExist("type"));

   //}
   //void ValidateUrl(BaseAppForm model)
   //{
   //   if (string.IsNullOrEmpty(model.Url))
   //   {
   //      ModelState.AddModelError(nameof(model.Url), ValidationMessages.Required("Url"));
   //   }
   //   if (!model.Url!.IsValidUrl())
   //   {
   //      ModelState.AddModelError(nameof(model.Url), ValidationMessages.WrongFormatOf("Url"));
   //   }

   //}
}
