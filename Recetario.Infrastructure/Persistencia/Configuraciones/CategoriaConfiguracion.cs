using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetario.Domain;

namespace Recetario.Infrastructure.Persistencia.Configuraciones;

public sealed class CategoriaConfiguracion : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> constructor)
    {
        constructor.HasKey(categoria => categoria.Id);
        constructor.Property(categoria => categoria.Nombre).HasMaxLength(100).IsRequired();
        constructor.HasIndex(categoria => categoria.Nombre).IsUnique();
        constructor.HasData(
            new Categoria { Id = 1, Nombre = "Desayuno" },
            new Categoria { Id = 2, Nombre = "Almuerzo" },
            new Categoria { Id = 3, Nombre = "Cena" },
            new Categoria { Id = 4, Nombre = "Postre" },
            new Categoria { Id = 5, Nombre = "Bebida" },
            new Categoria { Id = 6, Nombre = "Antojito" });
    }
}
