using src.Core.Services.Interfaces;
using src.Repositories.Interfaces;

namespace src.Core.Services.Implementations
{
    public class OrdenPagoService : IOrdenPagoService
    {
        private readonly IOrdenPagoRepository _ordenPagoRepository;

        public OrdenPagoService(IOrdenPagoRepository ordenPagoRepository)
        {
            _ordenPagoRepository = ordenPagoRepository;
        }
    }
}
