using System.Reflection;
using MassTransit;
using MassTransit.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.Common.Configs;
using SendEmailMassTransit.DataProcessor.Services;
using SendMassTransit.Domain.Entities;

namespace SendEmailMassTransit.DataProcessor
{
    public static class StartupHelpers
    {
        public static IServiceCollection RegisterConfigurationServices(this IServiceCollection service,
            HostBuilderContext context)
        {
            var queueSettings = new QueueSettings();
            context.Configuration.GetSection("QueueSettings").Bind(queueSettings);
            service.AddSingleton(queueSettings);

            return service;
        }

        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services)
        {
            services.AddTransient<IMailService, MailService>();
            return services;
        }

        public static IServiceCollection RegisterRepositoryServices(this IServiceCollection services,
            HostBuilderContext context)
        {
            services.AddDbContext<EmailDbContext>(opt =>
            {
                opt.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
            });
            return services;
        }

        public static IServiceCollection RegisterQueueServices(this IServiceCollection services,
            HostBuilderContext context)
        {
            var queueSettings = new QueueSettings();
            context.Configuration.GetSection("QueueSettings").Bind(queueSettings);

            services.AddMassTransit(c => { c.AddConsumer<MailChangedConsumer>(); });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(queueSettings.HostName, queueSettings.VirtualHost, h =>
                {
                    h.Username(queueSettings.UserName);
                    h.Password(queueSettings.Password);
                });
                // cfg.UseExtensionsLogging(provider.GetService<ILoggerFactory>());
            }));
            services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());

            return services;
        }
    }
}