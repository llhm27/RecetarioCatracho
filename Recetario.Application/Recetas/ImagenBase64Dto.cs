using System.ComponentModel.DataAnnotations;

namespace Recetario.Application.Recetas;

public sealed class ImagenBase64Dto
{
    [Required(ErrorMessage = "El nombre del archivo es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre del archivo no puede superar los 255 caracteres.")]
    public string NombreArchivo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La imagen en Base64 es obligatoria.")]
    public string ImagenBase64 { get; set; } = string.Empty;
}
