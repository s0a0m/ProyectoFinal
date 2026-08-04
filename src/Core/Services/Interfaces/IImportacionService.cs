using System.Collections.Generic;
using System.Threading.Tasks;
using src.Contracts;
using Dom = src.Models.Domain;

namespace src.Core.Services.Interfaces
{
    public interface IImportacionService
    {
        /// <summary>
        /// Procesa un stream de Excel, valida reglas de negocio y actualiza/crea productos o novedades.
        /// Utiliza procesamiento por lotes para optimizar el rendimiento.
        /// </summary>
        IAsyncEnumerable<AccionDeFilaCargaAutomatica> ProcesarListaDePreciosAsync(
            Stream fileStream,
            short idProveedor,
            ImportacionColumnaMap mapaColumnas,
            bool contieneEncabezado,
            CancellationToken cancellationToken);
    }
}