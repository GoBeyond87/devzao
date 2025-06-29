using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data;

// Configuração da entidade Produto
internal class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
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
            
        builder.Property(p => p.DataCriacao)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
            
        builder.Property(p => p.DataAtualizacao)
            .IsRequired(false);
    }
}

// Configuração da entidade User
internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.HasIndex(u => u.Email).IsUnique();
            
        builder.Property(u => u.PasswordHash)
            .IsRequired();
            
        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");
    }
}
