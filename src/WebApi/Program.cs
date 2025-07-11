using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using WebApi.Messaging;

// Configura a cultura para invariante para evitar problemas de globalização
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

// Configuração do logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Adiciona serviços ao container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configuração do DbContext
try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
    
    Console.WriteLine($"Using database connection string: {connectionString}");
    
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString, 
            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                maxRetryCount: 5, 
                maxRetryDelay: TimeSpan.FromSeconds(30), 
                errorNumbersToAdd: null)));
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao configurar o banco de dados: {ex.Message}");
    throw;
}

// Configuração do Redis
try
{
    var redisConnection = builder.Configuration.GetConnectionString("Redis");
    if (!string.IsNullOrEmpty(redisConnection))
    {
        Console.WriteLine($"Configurando Redis com a string de conexão: {redisConnection}");
        
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.InstanceName = "CleanArch_" + Environment.MachineName + "_";
        });

        // Registrar o serviço de cache
        builder.Services.AddScoped<ICacheService, Infrastructure.Services.RedisCacheService>();
        
        Console.WriteLine("Redis configurado com sucesso!");
    }
    else
    {
        Console.WriteLine("Atenção: String de conexão do Redis não encontrada. Usando cache em memória.");
        builder.Services.AddDistributedMemoryCache();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao configurar o Redis: {ex.Message}");
    Console.WriteLine("Continuando sem cache distribuído. Usando cache em memória.");
    builder.Services.AddDistributedMemoryCache();
}

// Configuração do AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Injeção de dependência
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Kafka Test",
        Version = "v1",
        Description = "API para teste de integração com Kafka",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@empresa.com"
        }
    });

    // Habilita anotações XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configuração do Kafka
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddHostedService<KafkaConsumerService>();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Configura o Swagger apenas em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Kafka Test v1");
        c.RoutePrefix = "swagger"; // Define a rota base do Swagger UI
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Criar banco de dados e aplicar migrações automaticamente
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    
    Console.WriteLine("Aplicando migrações...");
    context.Database.Migrate();
    Console.WriteLine("Migrações aplicadas com sucesso!");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
    throw;
}

Console.WriteLine("Iniciando a aplicação...");
app.Run();

// Classe parcial para referência em testes
public partial class Program { }
