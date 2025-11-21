using System.Collections.Generic;
using System.Threading.Tasks;
using Dom = src.Models.Domain;
using src.Presentation.ViewModels.NovedadesVM;

namespace src.Core.Services.Interfaces
{
    public interface INovedadesService
    {
        Task<IEnumerable<NovedadesListarViewModel>> GetAllNovedadesPendientes();
    }
}