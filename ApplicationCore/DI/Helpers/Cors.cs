using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Consts;
using Infrastructure.Helpers;
using System;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Services.Identity;

namespace ApplicationCore.DI;
public static class CorsDI
{
   public static async void AddCorsPolicy(this IServiceCollection services, ConfigurationManager Configuration)
   {
      string clientUrl = Configuration[$"{SettingsKeys.App}:ClientUrl"] ?? "";
      string adminUrl = Configuration[$"{SettingsKeys.App}:AdminUrl"] ?? "";

      
      if (String.IsNullOrEmpty(adminUrl))
      {
         throw new Exception("Failed Add Cors. Empty AdminUrl.");
      }


      AddCorsPolicy(services, clientUrl.SplitToList(), adminUrl.SplitToList());

   }

   static void AddCorsPolicy(IServiceCollection services, List<string> clientUrls, List<string> adminUrls)
   {
      services.AddCors(options =>
      {
         options.AddPolicy("Api",
         builder =>
         {
            builder.WithOrigins(clientUrls.Concat(adminUrls).ToArray())
                 .AllowAnyHeader()
                  .AllowAnyMethod();
         });

         options.AddPolicy("Admin",
         builder =>
         {
            builder.WithOrigins(adminUrls.ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod();
         });

         options.AddPolicy("Global",
         builder =>
         {
            builder.WithOrigins(clientUrls.Concat(adminUrls).ToArray())
                  .AllowAnyHeader()
                  .AllowAnyMethod();
         });

         options.AddPolicy("Open",
         builder =>
         {
            builder.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
         });
      });
   }
}
