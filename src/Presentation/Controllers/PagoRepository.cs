using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
// using src.Presentation.ViewModels.Pagos;

namespace src.Presentation.Controllers
{
    [Route("OrdenesPago")]
    public class OrdenesPagoController : Controller
    {
        private readonly IOrdenPagoService _ordenPagoService;

        public OrdenesPagoController(IOrdenPagoService ordenPagoService)
        {
            _ordenPagoService = ordenPagoService;
        }
    }
}
