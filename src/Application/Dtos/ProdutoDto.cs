using System.ComponentModel.DataAnnotations;

namespace Application.Dtos;

public class ProdutoDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Descricao { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "O campo {0} deve ser maior que zero")]
    public decimal Preco { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "O campo {0} não pode ser negativo")]
    public int QuantidadeEmEstoque { get; set; }
}

public class CriarProdutoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Descricao { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "O campo {0} deve ser maior que zero")]
    public decimal Preco { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "O campo {0} não pode ser negativo")]
    public int QuantidadeEmEstoque { get; set; }
}

public class AtualizarProdutoDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    public string? Descricao { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "O campo {0} deve ser maior que zero")]
    public decimal Preco { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "O campo {0} não pode ser negativo")]
    public int QuantidadeEmEstoque { get; set; }
}
