using Api.Shared;
using Api.Shared.Middlewares;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Shared.Log.Extensions;
using Shared.Mapping;
using Shared.ViewModel.AppSettings;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;

namespace Api.Users
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            // Current dependencies
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
             .AddJsonOptions(options =>
              {
                  options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
              })
              .AddXmlSerializerFormatters()
              .AddXmlDataContractSerializerFormatters();

            services.AddAutoMapper(new Assembly[] { typeof(DotNetExerciseProfile).Assembly });

            services.AddSingleton<IControllerActivator, ControllerActivator>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "DotNetExercise", Version = "v1" });
            });

            services.AddLogging();

            services.AddSingleton(new ConnectionSettings()
            {
                LocalDb = Configuration.GetConnectionString("LocalDb")
            });

            //Project dependencies
            Business.Users.Startup.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHttpRequestMiddleware();

            app.UseCors(options => options
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            );

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotNetExercise V1");
            });

            loggerFactory.AddLog4Net(Directory.GetCurrentDirectory() + "\\Settings\\log4net.config");

        }
    }
}
