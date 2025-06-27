using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(
        IProdutoService produtoService,
        ILogger<ProdutosController> logger)
    {
        _produtoService = produtoService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os produtos cadastrados
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var produtos = await _produtoService.ListarTodosAsync();
        return Ok(produtos);
    }

    /// <summary>
    /// Obtém um produto pelo seu ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        
        if (produto == null)
            return NotFound("Produto não encontrado");
            
        return Ok(produto);
    }

    /// <summary>
    /// Adiciona um novo produto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Adicionar([FromBody] CriarProdutoDto dto)
    {
        try
        {
            var produto = await _produtoService.AdicionarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Já existe um produto com este nome"))
        {
            _logger.LogWarning(ex, "Tentativa de adicionar produto com nome duplicado: {Nome}", dto.Nome);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar produto");
            return StatusCode(500, new { message = "Ocorreu um erro ao processar sua solicitação" });
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto a ser atualizado</param>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProdutoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoDto dto)
    {
        try
        {
            var produto = await _produtoService.AtualizarAsync(id, dto);
            return Ok(produto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Produto não encontrado");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("já existe"))
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto com ID {ProdutoId}", id);
            return BadRequest("Ocorreu um erro ao atualizar o produto");
        }
    }

    /// <summary>
    /// Remove um produto
    /// </summary>
    /// <param name="id">ID do produto a ser removido</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _produtoService.RemoverAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Produto não encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover produto com ID {ProdutoId}", id);
            return BadRequest("Ocorreu um erro ao remover o produto");
        }
    }
}
