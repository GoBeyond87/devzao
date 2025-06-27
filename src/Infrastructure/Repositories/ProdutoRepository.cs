using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoRepository(ApplicationDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Produto?> ObterPorIdAsync(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<IReadOnlyList<Produto>> ListarTodosAsync()
    {
        return await _context.Produtos
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Produto> AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _unitOfWork.CompleteAsync();
        return produto;
    }

    public async Task AtualizarAsync(Produto produto)
    {
        _context.Entry(produto).State = EntityState.Modified;
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoverAsync(Produto produto)
    {
        _context.Produtos.Remove(produto);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> ExisteProdutoComNomeAsync(string nome, int? idIgnorar = null)
    {
        return await _context.Produtos
            .AnyAsync(p => p.Nome == nome && (idIgnorar == null || p.Id != idIgnorar));
    }
}
