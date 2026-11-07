namespace Recetario.Domain;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Receta> Recetas { get; set; } = new List<Receta>();
}
