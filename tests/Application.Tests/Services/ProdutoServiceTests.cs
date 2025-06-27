using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProdutoService _service;

    public ProdutoServiceTests()
    {
        _mockRepository = new Mock<IProdutoRepository>();
        _mockMapper = new Mock<IMapper>();
        _service = new ProdutoService(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task ObterPorId_QuandoProdutoExiste_RetornaProduto()
    {
        // Arrange
        var produto = new Produto("Produto Teste", "Descrição Teste", 10.5m, 100);
        var produtoDto = new ProdutoDto { Id = 1, Nome = produto.Nome };
        
        _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(produto);
        _mockMapper.Setup(m => m.Map<ProdutoDto>(produto)).Returns(produtoDto);

        // Act
        var resultado = await _service.ObterPorIdAsync(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(1, resultado.Id);
        Assert.Equal("Produto Teste", resultado.Nome);
    }

    [Fact]
    public async Task Adicionar_QuandoDadosValidos_AdicionaProduto()
    {
        // Arrange
        var dto = new CriarProdutoDto { Nome = "Novo Produto", Descricao = "Descrição", Preco = 10.5m, QuantidadeEmEstoque = 100 };
        var produto = new Produto(dto.Nome, dto.Descricao, dto.Preco, dto.QuantidadeEmEstoque);
        var produtoDto = new ProdutoDto { Id = 1, Nome = dto.Nome };
        
        _mockRepository.Setup(r => r.ExisteProdutoComNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Produto>(dto)).Returns(produto);
        _mockMapper.Setup(m => m.Map<ProdutoDto>(produto)).Returns(produtoDto);
        _mockRepository.Setup(r => r.AdicionarAsync(It.IsAny<Produto>())).ReturnsAsync(produto);

        // Act
        var resultado = await _service.AdicionarAsync(dto);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Novo Produto", resultado.Nome);
        _mockRepository.Verify(r => r.AdicionarAsync(It.IsAny<Produto>()), Times.Once);
    }

    [Fact]
    public async Task Adicionar_QuandoNomeJaExiste_LancaExcecao()
    {
        // Arrange
        var dto = new CriarProdutoDto { Nome = "Produto Existente", Descricao = "Descrição", Preco = 10.5m, QuantidadeEmEstoque = 100 };
        
        _mockRepository.Setup(r => r.ExisteProdutoComNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(true);

        // Act & Assert
        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AdicionarAsync(dto));
            
        Assert.Equal("Já existe um produto com este nome", excecao.Message);
    }

    [Fact]
    public async Task Remover_QuandoProdutoExiste_RemoveProduto()
    {
        // Arrange
        var produto = new Produto("Produto Teste", "Descrição", 10.5m, 100);
        
        _mockRepository.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(produto);
        _mockRepository.Setup(r => r.RemoverAsync(produto)).Returns(Task.CompletedTask);

        // Act
        await _service.RemoverAsync(1);


        // Assert
        _mockRepository.Verify(r => r.ObterPorIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.RemoverAsync(produto), Times.Once);
    }
}
