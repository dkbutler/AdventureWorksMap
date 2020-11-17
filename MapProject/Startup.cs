using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MapProject.AWData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MapProject.Models;
using MapProject.Controllers;
using MapProject.Controllers.Interfaces;
using Microsoft.AspNetCore.Mvc.Razor;
using MapProject.Services.Interfaces;
using MapProject.Services;

namespace MapProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MapProjectAWDBContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("MapProjectDBConnection")));

            services.AddSwaggerGen();

            services.AddControllersWithViews();
            services.Configure<RazorViewEngineOptions>(o =>
            {
              o.ViewLocationFormats.Clear();
              o.ViewLocationFormats.Add
                ("/Views/{0}" + RazorViewEngine.ViewExtension);
              o.ViewLocationFormats.Add
                ("/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
            });

      services.AddScoped<IConversionServiceController, ConversionServiceController>();
    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AddCoordinatesToAddresses Monolithic");
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Map}/{action=GetMapPage}/{id?}");
            });
        }
    }
}
