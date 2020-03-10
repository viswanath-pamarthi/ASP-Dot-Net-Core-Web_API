using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;
using RestApi.Filters;
using RestApi.Models;
using Microsoft.EntityFrameworkCore;

namespace RestApi
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
            //this line of code it pulls the info section from appsettings and creates a new instance of HotelInfo
            // and wraps that instance in an interface called IOptions and puts ioptions instance in services container.
            //this means it can be injected in to controllers
            services.Configure<HotelInfo>(Configuration.GetSection("Info"));



            //use in-Memory database to quick  development and testing
            //TODO: Swap out for real database in production
            services.AddDbContext<HotelApiDbContext>(options => options.UseInMemoryDatabase("landondb"));

            services.AddControllers(option=>
            {
                option.Filters.Add<JsonExceptionFilter>();
                option.Filters.Add<RequireHttpsOrCloseAttribute>();//This will apply to entire api at once
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSwaggerDocument();

            services.AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(1, 0);//default version 1.0, if not specified

                //where to read api version
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;//will get api version in responses
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);//pass options to vfersion selector

            
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyApp", policy => policy.AllowAnyOrigin());//allow any origin is useful during development next commented part should be added for production in place of any wildcard      // policy => policy.WithOrigins("https://example.com"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseHsts();//asp.net core by default uses https, http connections are redirect to https. if to reject http connections by turning on Http strict transport security(HSTS)
            }

            app.UseCors("AllowMyApp");

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
