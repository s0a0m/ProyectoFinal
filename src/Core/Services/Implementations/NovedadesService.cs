using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;
using src.Presentation.ViewModels.FamiliaVM;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.NovedadesVM;
using src.Models.CodeFirst;
using src.Contracts;

namespace src.Core.Services.Implementations
{
    public class NovedadesService : INovedadesService
    {
        private readonly INovedadesRepository _novedadesRepo;
        private readonly IProveedorRepository _proveedorRepo;

        public NovedadesService(INovedadesRepository novedadesRepo, IProveedorRepository proveedorRepo)
        {
            _novedadesRepo = novedadesRepo;
            _proveedorRepo = proveedorRepo;
        }

        public async Task<IEnumerable<NovedadesListarViewModel>> GetAllNovedadesPendientes()
        {
            var efnovedades = await _novedadesRepo.GetPendientesAsync();
            var novedadesVM = await MapListarNovedadeVM(efnovedades);
            return novedadesVM;
        }

        private async Task<IEnumerable<NovedadesListarViewModel>> MapListarNovedadeVM(IEnumerable<NovedadPendiente> source)
        {
            var lista = new List<NovedadesListarViewModel>();

            foreach (var x in source)
            {
                lista.Add(new NovedadesListarViewModel
                {
                    IdNovedad = x.IdNovedad,
                    RazonSocialProveedor = await ObtenerRazonSocialProveedor(x.IdProveedor),
                    CodigoBarraExterno = x.CodigoBarraExterno,
                    NombreSugerido = x.NombreSugerido,
                    PrecioSugerido = x.PrecioSugerido
                });
            }

            return lista;
        }

        private async Task<string> ObtenerRazonSocialProveedor(short id)
        {
            var proveedor = await _proveedorRepo.GetProveedorById(id);
            return proveedor?.RazonSocial ?? "desconocido";
        }
    }
}