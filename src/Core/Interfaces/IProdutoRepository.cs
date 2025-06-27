using Core.Entities;

namespace Core.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(int id);
    Task<IReadOnlyList<Produto>> ListarTodosAsync();
    Task<Produto> AdicionarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task RemoverAsync(Produto produto);
    Task<bool> ExisteProdutoComNomeAsync(string nome, int? idIgnorar = null);
}
