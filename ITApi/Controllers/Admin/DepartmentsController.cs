using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views.Identity;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Services.Identity;

namespace ITApi.Controllers.Admin;

public class DepartmentsController : BaseAdminController
{
   private readonly IDepartmentsService _departmentsService;
   private readonly IMapper _mapper;

   public DepartmentsController(IDepartmentsService departmentsService, IMapper mapper)
   {
      _departmentsService = departmentsService; 
      _mapper = mapper;
   }

   [HttpGet]   
   public async Task<ActionResult<ICollection<DepartmentViewModel>>> Index()
   {
      var departments = await _departmentsService.FetchRootsAsync();
      departments = departments.Where(x => x.Active);
      departments = departments.GetOrdered();

      return departments.MapViewModelList(_mapper);
   }
}
