using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Application.Dtos;
using Application.ViewModels;

namespace WebApp.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public ProdutosController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001/api/";
        }

        // GET: Produtos
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.GetAsync("produtos");
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var produtos = JsonSerializer.Deserialize<List<ProdutoDto>>(content, options);
                
                return View(produtos);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar produtos: " + ex.Message;
                return View(new List<ProdutoDto>());
            }
        }

        // GET: Produtos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.GetAsync($"produtos/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                    
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var produto = JsonSerializer.Deserialize<ProdutoDto>(content, options);
                
                if (produto == null)
                {
                    TempData["ErrorMessage"] = "Erro ao carregar o produto: Dados inválidos recebidos da API.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(produto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar detalhes do produto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Produtos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CriarProdutoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(dto, options),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("produtos", jsonContent);
                
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = errorContent;
                    return View(dto);
                }
                
                response.EnsureSuccessStatusCode();
                
                TempData["SuccessMessage"] = "Produto criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao criar produto: " + ex.Message;
                return View(dto);
            }
        }

        // GET: Produtos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.GetAsync($"produtos/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                    
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var produto = JsonSerializer.Deserialize<ProdutoDto>(content, options);
                
                if (produto == null)
                {
                    TempData["ErrorMessage"] = "Erro ao carregar o produto: Dados inválidos recebidos da API.";
                    return RedirectToAction(nameof(Index));
                }
                
                var viewModel = new EditarProdutoViewModel
                {
                    Id = produto.Id,
                    Nome = produto.Nome,
                    Descricao = produto.Descricao,
                    Preco = produto.Preco,
                    QuantidadeEmEstoque = produto.QuantidadeEmEstoque
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar produto para edição: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Produtos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditarProdutoViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var dto = new AtualizarProdutoDto
                {
                    Id = viewModel.Id,
                    Nome = viewModel.Nome,
                    Descricao = viewModel.Descricao,
                    Preco = viewModel.Preco,
                    QuantidadeEmEstoque = viewModel.QuantidadeEmEstoque
                };

                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(dto, options),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PutAsync($"produtos/{id}", jsonContent);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                    
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = errorContent;
                    return View(viewModel);
                }
                
                response.EnsureSuccessStatusCode();
                
                TempData["SuccessMessage"] = "Produto atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao atualizar produto: " + ex.Message;
                return View(viewModel);
            }
        }

        // GET: Produtos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.GetAsync($"produtos/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                    
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var produto = JsonSerializer.Deserialize<ProdutoDto>(content, options);
                
                if (produto == null)
                {
                    TempData["ErrorMessage"] = "Erro ao carregar o produto: Dados inválidos recebidos da API.";
                    return RedirectToAction(nameof(Index));
                }
                
                return View(produto);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao carregar produto para exclusão: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Produtos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ProdutoApi");
                var response = await client.DeleteAsync($"produtos/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound();
                    
                response.EnsureSuccessStatusCode();
                
                TempData["SuccessMessage"] = "Produto excluído com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erro ao excluir produto: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
