using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);
                
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(128);
                
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(u => u.IsActive)
                .IsRequired();
                
            builder.Property(u => u.CreatedAt)
                .IsRequired();
                
            builder.Property(u => u.UpdatedAt)
                .IsRequired(false);
        }
    }
}
