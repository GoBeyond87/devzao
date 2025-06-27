using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    // Construtor para injeção de dependência
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    
    // Construtor sem parâmetros para migrações
    public ApplicationDbContext()
    {
    }

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurações das entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

// Configuração da entidade Produto
internal class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Descricao)
            .HasMaxLength(2000);
            
        builder.Property(p => p.Preco)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
            
        builder.Property(p => p.QuantidadeEmEstoque)
            .IsRequired();
            
        builder.Property(p => p.DataCriacao)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
    }
}
