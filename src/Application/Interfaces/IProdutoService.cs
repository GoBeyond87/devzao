using Application.Dtos;

namespace Application.Interfaces;

public interface IProdutoService
{
    Task<ProdutoDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<ProdutoDto>> ListarTodosAsync();
    Task<ProdutoDto> AdicionarAsync(CriarProdutoDto dto);
    Task<ProdutoDto> AtualizarAsync(int id, AtualizarProdutoDto dto);
    Task RemoverAsync(int id);
}
