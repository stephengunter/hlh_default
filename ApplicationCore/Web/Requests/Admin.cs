﻿using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Web.Requests;
public class AdminRequest
{
   public string Key { get; set; } = String.Empty;
   public string? Cmd { get; set; }
   public string? Token { get; set; }
   public string? Data { get; set; }
}
public class AdminFileRequest : AdminRequest
{
   public List<IFormFile> Files { get; set; } = new List<IFormFile>();

   public IFormFile? GetFile(string name)
   {
      if (Files.IsNullOrEmpty()) return null;
      return Files.FirstOrDefault(item => Path.GetFileNameWithoutExtension(item.FileName) == name);

   }
}