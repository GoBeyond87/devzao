using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebApi.Messaging;
using WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.InMemory;

namespace Application.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddJsonFile("appsettings.test.json", optional: true);
            });

            builder.ConfigureServices(services =>
            {
                var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("DefaultConnection") ?? 
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.test.json");
                // Remove o serviço de cache existente
                var descriptor = services.FirstOrDefault(
                    d => d.ServiceType == typeof(IDistributedCache));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona cache em memória para testes
                services.AddDistributedMemoryCache();

                // Remove o contexto de banco existente
                var contextDescriptor = services.FirstOrDefault(
                    d => d.ServiceType == typeof(ApplicationDbContext));
                if (contextDescriptor != null)
                {
                    services.Remove(contextDescriptor);
                }

                // Adiciona o contexto do banco de dados SQL Server para testes
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));

                // Remove o serviço de cache existente
                var cacheServiceDescriptor = services.FirstOrDefault(
                    d => d.ServiceType == typeof(ICacheService));
                if (cacheServiceDescriptor != null)
                {
                    services.Remove(cacheServiceDescriptor);
                }

                // Adiciona o serviço de cache em memória para testes
                services.AddScoped<ICacheService, Infrastructure.Services.MemoryCacheService>();
            });
        }
    }
}
