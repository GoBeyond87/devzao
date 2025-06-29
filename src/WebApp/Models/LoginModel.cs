using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using StackExchange.Redis;
using Core.Entities;
using Infrastructure.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
// Removendo dependências do Entity Framework
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace WebApp.Models
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConnectionMultiplexer _redis;

        public LoginModel(
            ILogger<LoginModel> logger,
            IHttpClientFactory httpClientFactory,
            IConnectionMultiplexer redis)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _redis = redis;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório")]
            [EmailAddress(ErrorMessage = "Por favor, insira um email válido")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A senha é obrigatória")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Inicializa as variáveis
            User? user = null;
            var db = _redis.GetDatabase();
            var cacheKey = $"user:{Input.Email.ToLower()}";
            var cachedUser = await db.StringGetAsync(cacheKey);

            if (!cachedUser.HasValue)
            {
                // Se não estiver no cache, consulta a API
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.PostAsync("users/authenticate", new StringContent(
                    JsonSerializer.Serialize(new { email = Input.Email, password = Input.Password }),
                    System.Text.Encoding.UTF8,
                    "application/json"
                ));
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<User>(content);

                    if (user != null)
                    {
                        // Cache o usuário por 1 hora
                        await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(user), TimeSpan.FromHours(1));
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro na autenticação: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
                    return Page();
                }
            }
            else
            {
                // Se estiver no cache, usa o usuário do cache
                user = JsonSerializer.Deserialize<User>(cachedUser.ToString());
            }

            if (user != null && user.IsActive)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Input.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                var principal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync("CookieAuthentication", principal, authProperties);

                _logger.LogInformation("Login realizado com sucesso para o usuário {Email}", Input.Email);
                return RedirectToPage("/Produtos/Index");
            }

            ModelState.AddModelError(string.Empty, "Email ou senha inválidos");
            return Page();
        }

        // Removendo a verificação local de senha, pois agora a validação é feita pela API
        // private bool VerifyPassword(string password, string hashedPassword)
        // {
        //     using var sha256 = SHA256.Create();
        //     var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //     return Convert.ToBase64String(hash) == hashedPassword;
        // }
    }
}
