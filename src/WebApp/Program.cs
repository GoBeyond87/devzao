using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Garante que o caminho para wwwroot seja absoluto
    WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
});

// Configura para usar apenas HTTP
builder.WebHost.UseUrls("http://localhost:5000");

// Adiciona serviços ao container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Configuração do HttpClient para chamadas à API
builder.Services.AddHttpClient("ProdutoApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:7000/api/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produtos}/{action=Index}/{id?}");

app.Run();
