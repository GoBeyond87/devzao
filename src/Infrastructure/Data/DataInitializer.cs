using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Core.Entities;
using System.Text.Json;

namespace Infrastructure.Data
{
    public class DataInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, IDistributedCache cache)
        {
            try
            {
                // Verifica se já existe algum usuário
                if (await context.Users.AnyAsync())
                    return;

                // Cria um usuário de teste
                var testUser = new User
                {
                    Name = "Usuário Teste",
                    Email = "teste@devzao.com",
                    PasswordHash = string.Empty, // Será definido abaixo
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                // Senha de teste: "123456"
                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                testUser.PasswordHash = Convert.ToBase64String(hash);

                context.Users.Add(testUser);
                await context.SaveChangesAsync();

                // Inicializa o Redis
                await cache.SetStringAsync("testUser", JsonSerializer.Serialize(testUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao verificar/inicializar dados: {ex.Message}");
                // Se houver erro, assume que o banco já está inicializado
                return;
            }
        }
    }
}
