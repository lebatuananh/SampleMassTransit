using System;
using System.IO;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using SendEmailMassTransit.API.ChangedMessage;
using SendEmailMassTransit.Common.Services;
using Serilog;

namespace SendEmailMassTransit.API.Helpers
{
    public static class StartupHelpers
    {
        public static IServiceCollection RegisterSwager(this IServiceCollection services)
        {
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Email API", Version = "v1"}); });

            return services;
        }

        public static IServiceCollection RegisterCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("DefaultPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            return services;
        }

        public static IServiceCollection RegisterConfigurationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<Appsettings>();
            services.Configure<Appsettings>(appSettingsSection);
            services.AddSingleton(appSettings);
            return services;
        }


        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IMailServices, MailServices>();
            return services;
        }

        public static IServiceCollection RegisterQueueServices(this IServiceCollection services, IConfiguration section)
        {
            var appSettingsSection = section.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<Appsettings>();

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(appSettings.QueueSettings.HostName, appSettings.QueueSettings.VirtualHost,
                    h =>
                    {
                        h.Username(appSettings.QueueSettings.UserName);
                        h.Password(appSettings.QueueSettings.Password);
                    });

                cfg.ExchangeType = ExchangeType.Direct;
            }));

            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            return services;
        }

        public static IServiceCollection RegisterLogging(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                    optional: true)
                .Build();

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

                Log.Information("WebApi Starting...");
            }
            catch (Exception ex)
            {
            }

            return services;
        }
        
        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email API V1");
            });

            return app;
        }
    }
}