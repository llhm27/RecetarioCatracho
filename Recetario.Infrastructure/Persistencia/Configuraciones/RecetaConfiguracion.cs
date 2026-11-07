using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recetario.Domain;

namespace Recetario.Infrastructure.Persistencia.Configuraciones;

public sealed class RecetaConfiguracion : IEntityTypeConfiguration<Receta>
{
    public void Configure(EntityTypeBuilder<Receta> constructor)
    {
        constructor.HasKey(receta => receta.Id);
        constructor.Property(receta => receta.Id).ValueGeneratedOnAdd();
        constructor.Property(receta => receta.Nombre).HasMaxLength(100).IsRequired();
        constructor.HasIndex(receta => receta.Nombre).IsUnique();
        constructor.Property(receta => receta.Dificultad).HasConversion<string>().IsRequired();
        constructor.Property(receta => receta.Ingredientes).HasMaxLength(2000).IsRequired();
        constructor.Property(receta => receta.Pasos).HasMaxLength(4000).IsRequired();
        constructor.Property(receta => receta.ImagenRuta).HasMaxLength(500);
        constructor.Property(receta => receta.VideoRuta).HasMaxLength(500);
        constructor.Property(receta => receta.FechaCreacion).IsRequired();
        constructor.HasOne(receta => receta.Categoria).WithMany(categoria => categoria.Recetas)
            .HasForeignKey(receta => receta.CategoriaId).OnDelete(DeleteBehavior.Restrict);
    }
}
