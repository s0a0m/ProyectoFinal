using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;

namespace src.Core.Services.Implementations
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IComprobanteRepository _comprobanteRepository;

        public ComprobanteService(IComprobanteRepository comprobanteRepository)
        {
            _comprobanteRepository = comprobanteRepository;
        }
    }
}
