using Microsoft.AspNetCore.Mvc;
using src.Core.Services.Interfaces;

namespace src.Presentation.ViewComponents
{
    public class CarritoViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;

        public CarritoViewComponent(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cantidad = await _cartService.GetCantidadTotalItemsAsync();
            return View(cantidad);
        }
    }
}