using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views.Identity;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using ApplicationCore.Services.Identity;
using ApplicationCore.Services;

namespace ITApi.Controllers.Admin;

public class UsersController : BaseAdminController
{
   private readonly IAppUsersService _usersService;
   private readonly IMapper _mapper;

   public UsersController(IAppUsersService usersService, IMapper mapper)
   {
      _usersService = usersService; 
      _mapper = mapper;
   }

   [HttpGet]   
   public async Task<ActionResult<ICollection<UserViewModel>>> Index()
   {
      bool includeRoles = true;
      var users = await _usersService.FetchAllAsync(includeRoles);
      users = users.Where(u => u.Active);
      return users.MapViewModelList(_mapper);
   }
}
