using System.ComponentModel.DataAnnotations;
using Recetario.Domain;

namespace Recetario.Application.Recetas;

public sealed class CrearRecetaDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "La categoría es obligatoria.")]
    public int CategoriaId { get; set; }

    [EnumDataType(typeof(Dificultad), ErrorMessage = "La dificultad no es válida.")]
    public Dificultad Dificultad { get; set; }

    [Range(1, 600, ErrorMessage = "El tiempo de preparación debe estar entre 1 y 600 minutos.")]
    public int TiempoPreparacionMin { get; set; }

    [Range(1, 50, ErrorMessage = "Las porciones deben estar entre 1 y 50.")]
    public int Porciones { get; set; }

    [Required(ErrorMessage = "Los ingredientes son obligatorios.")]
    [StringLength(2000, ErrorMessage = "Los ingredientes no pueden superar los 2000 caracteres.")]
    public string Ingredientes { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los pasos son obligatorios.")]
    [StringLength(4000, ErrorMessage = "Los pasos no pueden superar los 4000 caracteres.")]
    public string Pasos { get; set; } = string.Empty;
}
