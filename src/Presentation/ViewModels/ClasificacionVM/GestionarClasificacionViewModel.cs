using src.Models.Domain; // Para usar las entidades de dominio en la lista
using System.Collections.Generic;

namespace src.Presentation.ViewModels.ClasificacionVM
{
    public class GestionarClasificacionViewModel
    {
        // Lista de todas las familias (para el menú lateral o árbol)
        public IEnumerable<Familia> Familias { get; set; } = new List<Familia>();

        // Lista de categorías filtradas (solo se llena si IdFamiliaSeleccionada tiene valor)
        public IEnumerable<Categoria> CategoriasSeleccionadas { get; set; } = new List<Categoria>();

        // Estado de la UI: ¿Qué familia está viendo el usuario actualmente?
        public short? IdFamiliaSeleccionada { get; set; }

        // Propiedades opcionales para mostrar mensajes de error/éxito específicos en la vista
        // sin depender solo de TempData si quieres validaciones inline.
    }
}