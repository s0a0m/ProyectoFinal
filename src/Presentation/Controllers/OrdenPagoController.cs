using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;

namespace src.Presentation.Controllers
{
    [Route("OrdenPago")]
    public class OrdenPagoController : Controller
    {
        private readonly IOrdenPagoService _ordenPagoService;

        public OrdenPagoController(IOrdenPagoService ordenPagoService)
        {
            _ordenPagoService = ordenPagoService;
        }
    }
}
