using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.CategoriaVM;
using Dom = src.Models.Domain;

namespace src.Core.Services.Implementations
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly IFamiliaRepository _familiaRepo; // Necesario para validar que la familia exista

        public CategoriaService(ICategoriaRepository categoriaRepo, IFamiliaRepository familiaRepo)
        {
            _categoriaRepo = categoriaRepo;
            _familiaRepo = familiaRepo;
        }

        public Task<IEnumerable<Dom.Categoria>> GetAllAsync()
        {
            return _categoriaRepo.GetAllAsync();
        }

        public Task<IEnumerable<Dom.Categoria>> GetByFamiliaIdAsync(short idFamilia)
        {
            return _categoriaRepo.GetByFamiliaIdAsync(idFamilia);
        }

        public async Task<Dom.Categoria> GetByIdAsync(short id)
        {
            var categoria = await _categoriaRepo.GetByIdAsync(id);
            if (categoria == null)
                throw new KeyNotFoundException($"Categoría con ID {id} no encontrada.");
            
            return categoria;
        }

        public async Task<Dom.Categoria> CreateAsync(CrearCategoriaViewModel vm)
        {
            // Validar que la familia exista
            var familia = await _familiaRepo.GetByIdAsync(vm.IdFamilia);
            if (familia == null)
                throw new ArgumentException("La familia seleccionada no existe.");
            if (await _categoriaRepo.ExistsNombreEnFamiliaAsync(vm.Nombre,vm.IdFamilia)) 
            throw new ArgumentException("El nombre de la categoria ya existe en la familia que se intenta agregar, por favor ingrese uno que no se repita");
            // Mapeo manual VM -> Dominio
            // IMPORTANTE: Instanciamos la propiedad 'Familia' con el ID para que el Mapper del Repo funcione
            var nuevaCategoria = new Dom.Categoria
            {
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion?.Trim() ?? string.Empty,
                Familia = new Dom.Familia { IdFamilia = vm.IdFamilia } 
            };

            await _categoriaRepo.AddAsync(nuevaCategoria);
            return nuevaCategoria;
        }

        public async Task UpdateAsync(ActualizarCategoriaViewModel vm)
        {
            var categoriaExistente = await _categoriaRepo.GetByIdAsync(vm.IdCategoria);
            if (categoriaExistente == null)
                throw new KeyNotFoundException($"Categoría con ID {vm.IdCategoria} no encontrada.");
            
            if (await _categoriaRepo.ExistsNombreEnFamiliaAsync(vm.Nombre,vm.IdFamilia,vm.IdCategoria)) 
            throw new ArgumentException("El nombre de la categoria ya existe en la familia que se intenta Modificar, por favor ingrese uno que no se repita");

            short idFamiliaActual = categoriaExistente.Familia?.IdFamilia ?? 0;
            // Validar si cambió de familia y si la nueva existe
            if (idFamiliaActual != vm.IdFamilia)
            {
                 var nuevaFamilia = await _familiaRepo.GetByIdAsync(vm.IdFamilia);
                 if (nuevaFamilia == null) throw new ArgumentException("La nueva familia seleccionada no existe.");
                 
                 // Actualizamos la referencia
                 categoriaExistente.Familia = new Dom.Familia { IdFamilia = vm.IdFamilia };
            }

            categoriaExistente.Nombre = vm.Nombre.Trim();
            categoriaExistente.Descripcion = vm.Descripcion?.Trim() ?? string.Empty;

            await _categoriaRepo.UpdateAsync(categoriaExistente);
        }

        public async Task DeleteAsync(short id)
        {
           await _categoriaRepo.DeleteAsync(id);
        }
    }
}