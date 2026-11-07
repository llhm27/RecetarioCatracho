namespace Recetario.Domain;

public class Receta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public Dificultad Dificultad { get; set; }
    public int TiempoPreparacionMin { get; set; }
    public int Porciones { get; set; }
    public string Ingredientes { get; set; } = string.Empty;
    public string Pasos { get; set; } = string.Empty;
    public string? ImagenRuta { get; set; }
    public string? VideoRuta { get; set; }
    public DateTime FechaCreacion { get; set; }

    public void ValidarReglas()
    {
        if (TiempoPreparacionMin is < 1 or > 600)
            throw new DomainException("El tiempo de preparación debe estar entre 1 y 600 minutos.");
        if (Porciones is < 1 or > 50)
            throw new DomainException("Las porciones deben estar entre 1 y 50.");
    }
}
