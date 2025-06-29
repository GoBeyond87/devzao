using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using StackExchange.Redis;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Garante que o caminho para wwwroot seja absoluto
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
});

// Configura para usar apenas HTTP
builder.WebHost.UseUrls("http://localhost:5000");

// Adiciona serviços ao container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Produtos");
});

// Configuração do Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
    });

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Configuração do Entity Framework (desativada para usar apenas a API)
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do HttpClient para chamadas à API
builder.Services.AddHttpClient("ProdutoApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:7000/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Removendo a inicialização do banco de dados local
// A aplicação agora depende apenas da API externa

// Configura o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Desabilita o redirecionamento HTTPS
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

// Configuração de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Protege todas as rotas de produtos
app.MapControllerRoute(
    name: "produtos",
    pattern: "Produtos/{action=Index}/{id?}")
    .RequireAuthorization();

// Adiciona a rota para a página de login
app.MapRazorPages();

// Configura a rota padrão para produtos
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}")
    .RequireAuthorization();

// Adiciona a página de login como página inicial
app.MapFallbackToPage("/Login");

app.Run();
