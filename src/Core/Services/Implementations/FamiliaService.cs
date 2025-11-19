using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.FamiliaVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class FamiliaService : IFamiliaService
    {
        private readonly IFamiliaRepository _familiaRepo;

        public FamiliaService(IFamiliaRepository familiaRepo)
        {
            _familiaRepo = familiaRepo;
        }

        public Task<IEnumerable<Dom.Familia>> GetAllAsync()
        {
            return _familiaRepo.GetAllAsync();
        }

        public Task<IEnumerable<Dom.Familia>> GetAllWithCategoriasAsync()
        {
            return _familiaRepo.GetAllWithCategoriasAsync();
        }

        public async Task<Dom.Familia> GetByIdAsync(short id)
        {
            var familia = await _familiaRepo.GetByIdAsync(id);
            if (familia == null)
                throw new KeyNotFoundException($"Familia con ID {id} no encontrada.");
            
            return familia;
        }

        public async Task<Dom.Familia> CreateAsync(CrearFamiliaViewModel vm)
        {
            // Mapeo manual ViewModel -> Dominio
            var nuevaFamilia = new Dom.Familia
            {
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion?.Trim() ?? string.Empty
            };

            await _familiaRepo.AddAsync(nuevaFamilia);
            return nuevaFamilia; // Retorna con el ID generado
        }

        public async Task UpdateAsync(ActualizarFamiliaViewModel vm)
        {
            // Verificamos existencia (opcional si el repo ya lanza excepción, pero buena práctica)
            var familiaExistente = await _familiaRepo.GetByIdAsync(vm.IdFamilia);
            if (familiaExistente == null)
                throw new KeyNotFoundException($"Familia con ID {vm.IdFamilia} no encontrada.");

            // Actualizamos la entidad de dominio
            familiaExistente.Nombre = vm.Nombre.Trim();
            familiaExistente.Descripcion = vm.Descripcion?.Trim() ?? string.Empty;

            await _familiaRepo.UpdateAsync(familiaExistente);
        }

        public async Task DeleteAsync(short id)
        {
            // 1. REGLA DE NEGOCIO: No borrar si tiene hijos
            bool tieneHijos = await _familiaRepo.HasCategoriasAsync(id);
            if (tieneHijos)
            {
                throw new InvalidOperationException("No se puede eliminar la familia porque tiene categorías asociadas. Elimine o mueva las categorías primero.");
            }

            // 2. Si pasa la regla, borrar
            await _familiaRepo.DeleteAsync(id);
        }
    }
}