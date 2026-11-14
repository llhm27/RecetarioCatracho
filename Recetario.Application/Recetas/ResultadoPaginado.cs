namespace Recetario.Application.Recetas;

public sealed record ResultadoPaginado<T>(IReadOnlyList<T> Elementos, int Total, int Pagina, int TamanoPagina);
