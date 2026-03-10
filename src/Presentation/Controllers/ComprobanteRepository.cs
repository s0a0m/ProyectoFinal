using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;
// using src.Presentation.ViewModels.Comprobantes; 

namespace src.Presentation.Controllers
{
    [Route("Comprobantes")]
    public class ComprobantesController : Controller
    {
        private readonly IComprobanteService _comprobanteService;
  
        public ComprobantesController(IComprobanteService comprobanteService)
        {
            _comprobanteService = comprobanteService;
        }

    }
}
