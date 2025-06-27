using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;

namespace Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public ProdutoService(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<ProdutoDto?> ObterPorIdAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null)
            return null;
            
        return _mapper.Map<ProdutoDto>(produto);
    }

    public async Task<IEnumerable<ProdutoDto>> ListarTodosAsync()
    {
        var produtos = await _produtoRepository.ListarTodosAsync();
        return _mapper.Map<IEnumerable<ProdutoDto>>(produtos);
    }

    public async Task<ProdutoDto> AdicionarAsync(CriarProdutoDto dto)
    {
        if (await _produtoRepository.ExisteProdutoComNomeAsync(dto.Nome))
            throw new InvalidOperationException("Já existe um produto com este nome");
            
        var produto = _mapper.Map<Produto>(dto);
        
        await _produtoRepository.AdicionarAsync(produto);
        
        return _mapper.Map<ProdutoDto>(produto);
    }

    public async Task<ProdutoDto> AtualizarAsync(int id, AtualizarProdutoDto dto)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado");
            
        if (await _produtoRepository.ExisteProdutoComNomeAsync(dto.Nome, id))
            throw new InvalidOperationException("Já existe outro produto com este nome");
        
        produto.Atualizar(dto.Nome, dto.Descricao ?? string.Empty, dto.Preco, dto.QuantidadeEmEstoque);
        
        await _produtoRepository.AtualizarAsync(produto);
        
        return _mapper.Map<ProdutoDto>(produto);
    }

    public async Task RemoverAsync(int id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado");
            
        await _produtoRepository.RemoverAsync(produto);
    }
}
