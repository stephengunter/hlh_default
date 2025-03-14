using Microsoft.AspNetCore.Mvc;
using IdentityWeb.Models;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using ApplicationCore.Authorization;
using ApplicationCore.Services.Identity;
using ApplicationCore.Models.Identity;
using ApplicationCore.Views.Identity;
using Ardalis.Specification;
using AutoMapper;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Helpers.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using System.Security.Principal;
using ApplicationCore.DataAccess;
using System.Linq;
using ApplicationCore.Models.IT;

namespace IdentityWeb.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly IAppService _appService;
   private readonly IMapper _mapper;

   public AATestsController(IAppService appService, IMapper mapper)
   {
      _appService = appService;
      _mapper = mapper;
   }
   [HttpGet]
   public async Task<ActionResult> Index()
   {
      
      return Ok();
   }
   //[HttpGet]
   //public async Task<ActionResult> Index()
   //{
   //   var items = _context.Items.ToList();
   //   var dicts = new Dictionary<string, int>();
   //   dicts.Add("AD201", 2);
   //   dicts.Add("AD220", 3);
   //   dicts.Add("AD230", 1);
   //   dicts.Add("AD243", 2);
   //   dicts.Add("AD300", 1);

   //   dicts.Add("AD301", 2);
   //   dicts.Add("AD310", 1);
   //   dicts.Add("AD380", 0);
   //   dicts.Add("AD381", 0);
   //   dicts.Add("AD382", 0);

   //   dicts.Add("AD383", 1);
   //   dicts.Add("AD400", 4);
   //   dicts.Add("AD401", 0);
   //   dicts.Add("AD405", 2);
   //   dicts.Add("AD410", 1);

   //   dicts.Add("AD420", 4);
   //   dicts.Add("AD425", 5);
   //   dicts.Add("AD500", 0);
   //   dicts.Add("AD501", 6);
   //   dicts.Add("AD507", 3);

   //   dicts.Add("AD520", 0);
   //   dicts.Add("AD602", 2);
   //   dicts.Add("AD605", 3);
   //   dicts.Add("AD607", 5);
   //   dicts.Add("AD712", 1);

   //   foreach (var kvp in dicts)
   //   {
   //      var item = items.FirstOrDefault(x => x.Code == kvp.Key);
   //      var bs = new ItemBalanceSheet
   //      {
   //         Date = new DateTime(2024, 12, 31),
   //         ItemId = item.Id,
   //         InQty = kvp.Value,
   //         Ps = "前期結存"
   //      };
   //      _context.ItemBalanceSheets.Add(bs);
   //   }
   //   //var codes = new List<string>()
   //   //{ 
   //   //   "AD201","AD220","AD230","AD243","AD300",
   //   //   "AD301","AD310","AD380","AD381","AD382",
   //   //   "AD383","AD400","AD401","AD405","AD410",
   //   //   "AD420","AD425","AD500","AD501","AD507",
   //   //   "AD520","AD602","AD605","AD607","AD712",
   //   //};
   //   //var items = _context.Items.ToList();
   //   //foreach (var item in items)
   //   //{
   //   //   if (codes.Contains(item.Code)) item.Order = 0;
   //   //   else item.Order = -1;
   //   //}
   //   _context.SaveChanges();
   //   //var codes = new List<string>()
   //   //{
   //   //   "AE001","AE002","AE003"
   //   //};
   //   return Ok(dicts);   
   //}
}
