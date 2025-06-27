using System;
using System.Linq;
using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Infrastructure.Services;

namespace Application.Tests.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remover o Redis real se estiver registrado
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ICacheService));
                
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adicionar o Redis em memória para testes
                services.AddDistributedMemoryCache();
                
                // Registrar o serviço de cache
                services.TryAddScoped<ICacheService, RedisCacheService>();
            });

            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
            });

            builder.UseEnvironment("Test");
        }
    }
}
