﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Formatters;
using Multilinks.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Multilinks.Core.Filters;
using Multilinks.Core.Models;
using Microsoft.EntityFrameworkCore;
using Multilinks.Core.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Multilinks.Core.Hubs;
using Multilinks.Core.Infrastructure.Security;
using System;

namespace Multilinks.Core
{
   public class Startup
   {
      private IConfiguration _configuration { get; }
      private IHostingEnvironment _env { get; }

      public Startup(IConfiguration configuration, IHostingEnvironment env)
      {
         _configuration = configuration;
         _env = env;
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddDbContext<CoreDbContext>(options =>
             options.UseSqlServer(_configuration.GetConnectionString("CoreDb")));

         services.AddAutoMapper();

         services.AddMvcCore()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddAuthorization()
            .AddJsonFormatters()
            .AddDataAnnotations()
            .AddMvcOptions(opt =>
            {
               var jsonFormatter = opt.OutputFormatters.OfType<JsonOutputFormatter>().Single();
               opt.OutputFormatters.Remove(jsonFormatter);
               opt.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));

               opt.Filters.Add(typeof(JsonExceptionFilter));
               opt.Filters.Add(typeof(LinkRewritingFilter));

               if(!_env.IsProduction())
               {
                  var launchJsonConfig = new ConfigurationBuilder()
                        .SetBasePath(_env.ContentRootPath)
                        .AddJsonFile("Properties\\launchSettings.json", optional: true)
                        .Build();
                  opt.SslPort = launchJsonConfig.GetValue<int>("iisSettings:iisExpress:sslPort");
               }
               opt.Filters.Add(new RequireHttpsAttribute());

               opt.CacheProfiles.Add("Static", new CacheProfile { Duration = 86400 });
               opt.CacheProfiles.Add("Collection", new CacheProfile { Duration = 5 });
               opt.CacheProfiles.Add("Resource", new CacheProfile { Duration = 10 });
            });

         services.AddRouting(opt => opt.LowercaseUrls = true);

         services.AddApiVersioning(opt =>
         {
            opt.ApiVersionReader = new MediaTypeApiVersionReader();
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
         });

         services.Configure<PagingOptions>(_configuration.GetSection("DefaultPagingOptions"));

         services.AddAuthentication(options =>
         {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         })
         .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
         {
            options.Authority = _configuration.GetValue<string>("IdentityConfig:AuthorityUrl");
            options.ApiName = _configuration.GetValue<string>("IdentityConfig:ApiName");
            options.TokenRetriever = CustomTokenRetriever.FromHeaderAndQueryString;
         });

         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         services.AddScoped<IUserInfoService, UserInfoService>();
         services.AddScoped<IHubConnectionService, HubConnectionService>();
         services.AddScoped<IEndpointService, EndpointService>();
         services.AddScoped<IEndpointLinkService, EndpointLinkService>();
         services.AddScoped<INotificationService, NotificationService>();

         services.AddSignalR(hubOptions =>
         {
            hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(20);
            hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
         });

         /* TODO: CORS policy will need to be updated before deployment. */
         services.AddCors(options =>
         {
            options.AddPolicy("CorsMyOrigins", builder =>
            {
               builder.WithOrigins(_configuration.GetValue<string>("Cors:Core"),
                            _configuration.GetValue<string>("Cors:Portal"))
               .AllowAnyMethod()
               .AllowCredentials()
               .AllowAnyHeader();
            });
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app)
      {
         if(_env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }

         app.UseHsts(opt =>
         {
            opt.MaxAge(days: 365);
            opt.IncludeSubdomains();
            opt.Preload();
         });

         app.UseCors("CorsMyOrigins");

         app.UseAuthentication();

         app.UseMvc();

         app.UseSignalR(routes =>
         {
            routes.MapHub<MainHub>("/hub/main");
         });
      }
   }
}
