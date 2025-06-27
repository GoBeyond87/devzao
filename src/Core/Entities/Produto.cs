namespace Core.Entities;

public class Produto
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public int QuantidadeEmEstoque { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    public Produto(string nome, string descricao, decimal preco, int quantidadeEmEstoque)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        QuantidadeEmEstoque = quantidadeEmEstoque;
        DataCriacao = DateTime.UtcNow;
    }

    public void Atualizar(string nome, string descricao, decimal preco, int quantidadeEmEstoque)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        QuantidadeEmEstoque = quantidadeEmEstoque;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
            
        if (QuantidadeEmEstoque < quantidade)
            throw new InvalidOperationException("Quantidade em estoque insuficiente");
            
        QuantidadeEmEstoque -= quantidade;
    }
}
