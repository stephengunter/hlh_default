using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using ApplicationCore.Web.Controllers;
using ApplicationCore.Services.IT;
using System.Text;
using Infrastructure.Helpers;
using ApplicationCore.Models.IT;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Hosting.Server;
using ApplicationCore.Helpers;
using AutoMapper;
using System.ComponentModel;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ApplicationCore.DataAccess;

namespace ITApi.Controllers.Tests;

public class AATestsController : BaseTestController
{
   private readonly DefaultContext _context;
   private readonly IMapper _mapper;
   public AATestsController(DefaultContext context, IMapper mapper)
   {
      _context = context;
      _mapper = mapper;
   }


   [HttpGet]
   public async Task<ActionResult> Index()
   {
      

      return Ok();
   }
}
