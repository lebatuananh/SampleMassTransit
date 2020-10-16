using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.API.Helpers;
using SendEmailMassTransit.API.Middlewares;
using SendMassTransit.Domain.Entities;
using Serilog;

namespace SendEmailMassTransit.API
{
    public class Startup
    {
        public static readonly string AssemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EmailDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => { b.MigrationsAssembly(AssemblyName); });
            });
            services.RegisterSwager().RegisterConfigurationServices(Configuration).RegisterCorsPolicy()
                .RegisterQueueServices(Configuration).RegisterServices().RegisterLogging();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("DefaultPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSwaggerConfiguration();
            app.UseAuthorization();
            loggerFactory.AddSerilog();
            app.UseMiddleware(typeof(ApiResponseMiddleware));
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}