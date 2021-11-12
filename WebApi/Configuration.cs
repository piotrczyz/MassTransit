using System;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi
{
    public static class DependencyInjections
    {
        public static void AddCoreMessaging(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies, bool isDevelopment, Action<IServiceCollectionBusConfigurator> configure = null)
        {
            var connectionString = configuration["Azure:ServiceBus:ConnectionString"];
            var queueName = configuration["Azure:ServiceBus:QueueName"];

            if (isDevelopment == false && (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName)))
            {
                throw new ArgumentNullException(nameof(AddCoreMessaging));
            }

            services.AddMediator(cfg =>
            {
                cfg.AddConsumers(assemblies);
            });
            
            services.AddMassTransit(x =>
            {
                x.AddConsumers(assemblies);

                //custom configuration per a microservice
                configure?.Invoke(x);

                if (isDevelopment)
                {
                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseMessageRetry(r =>
                        {
                            r.Immediate(5);
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                }
                else
                {
                  // message broker
                }
               
            });
            services.AddMassTransitHostedService();
            services.AddGenericRequestClient();
        }
    }
}