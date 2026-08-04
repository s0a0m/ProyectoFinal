using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using src.Infrastructure.Repositories;
using src.Repositories.Interfaces;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;

namespace src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PruebaRepoApiController : ControllerBase
{
    private readonly IComprobanteRepository _comprobanteRepo;
    private readonly IOrdenPagoRepository _pagoRepo;
    private readonly IFacturaRepository _facturaRepo;

    public PruebaRepoApiController(
        IComprobanteRepository comprobanteRepo,
        IOrdenPagoRepository pagoRepo,
        IFacturaRepository facturaRepo
    )
    {
        _comprobanteRepo = comprobanteRepo;
        _pagoRepo = pagoRepo;
        _facturaRepo = facturaRepo;
    }

    // ============================================================
    // 1. MOTIVOS (Datos Maestros)
    // ============================================================

    [HttpGet("motivos/{tipo}")]
    public async Task<IActionResult> GetMotivos(string tipo)
    {
        // Prueba: /api/test-tesoreria/motivos/NC
        var motivos = await _comprobanteRepo.GetMotivosAsync(tipo);
        return Ok(motivos);
    }

    [HttpGet("motivos")] // Opción sin parámetro para traer todos
    public async Task<IActionResult> GetAllMotivos()
    {
        var motivos = await _comprobanteRepo.GetMotivosAsync(null);
        return Ok(motivos);
    }

    // ============================================================
    // 2. COMPROBANTES (Lectura)
    // ============================================================

    [HttpGet("comprobantes/{id}")]
    public async Task<IActionResult> GetComprobanteById(int id)
    {
        var comp = await _comprobanteRepo.GetByIdAsync(id);
        if (comp == null)
            return NotFound($"No se encontró comprobante con ID {id}");

        // Debería traer Proveedor, Motivo y CondicionPago llenos
        return Ok(comp);
    }

    [HttpGet("notas-credito")]
    public async Task<IActionResult> GetAllNotasCredito()
    {
        // Debería traer la lista y dentro de cada una la "FacturaOriginal"
        var notas = await _comprobanteRepo.GetNotasCreditoAsync();
        return Ok(notas);
    }

    [HttpGet("notas-debito")]
    public async Task<IActionResult> GetAllNotasDebito()
    {
        var notas = await _comprobanteRepo.GetNotasDebitoAsync();
        return Ok(notas);
    }

    // ============================================================
    // 3. PAGOS (Lectura)
    // ============================================================

    [HttpGet("pagos")]
    public async Task<IActionResult> GetPagos()
    {
        // Lista simple de pagos (encabezados)
        var pagos = await _pagoRepo.GetAllAsync();
        return Ok(pagos);
    }

    [HttpGet("pagos/{id}")]
    public async Task<IActionResult> GetPagoById(int id)
    {
        // Este es importante para ver si trae los DETALLES y las FACTURAS pagadas
        var pago = await _pagoRepo.GetByIdWithDetallesAsync(id);

        if (pago == null)
            return NotFound();
        return Ok(pago);
    }

    [HttpPost("nota-credito")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNotaCredito([FromBody] CreateNotaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 1. Mapeo Manual DTO -> Dominio (NotaCredito)
            var nuevaNota = new Dom.NotaCredito
            {
                // IdProveedor = dto.IdProveedor,
                // IdCondicionPagoUsada = dto.IdCondicionPago,
                // IdMotivo = dto.IdMotivo,
                // IdFacturaReferencia = dto.IdFacturaReferencia,
                Numero = dto.Numero,
                Total = dto.Total,
                FechaEmision = DateTime.UtcNow, // Siempre UTC al guardar
                Comentario = dto.Comentario,
            };

            // 2. Llamada al Repo (Polimorfismo: AddAsync acepta Comprobante)
            var resultado = await _comprobanteRepo.AddAsync(nuevaNota);

            // 3. Respuesta 201 Created
            // 'GetComprobanteById' es el nombre del método GET que busca por ID
            return CreatedAtAction(
                nameof(GetComprobanteById),
                new { id = resultado.IdComprobante },
                resultado
            );
        }
        catch (Exception ex)
        {
            // Loguear error real en consola para debug
            Console.WriteLine(ex.ToString());
            return StatusCode(
                500,
                new { mensaje = "Error al crear Nota de Crédito", error = ex.Message }
            );
        }
    }

    // ============================================================
    // CREAR NOTA DE DÉBITO (POST)
    // ============================================================
    [HttpPost("nota-debito")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateNotaDebito([FromBody] CreateNotaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 1. Mapeo Manual DTO -> Dominio (NotaDebito)
            var nuevaNota = new Dom.NotaDebito
            {
                // IdProveedor = dto.IdProveedor,
                // IdCondicionPagoUsada = dto.IdCondicionPago,
                // IdMotivo = dto.IdMotivo,
                // IdFacturaReferencia = dto.IdFacturaReferencia,
                Numero = dto.Numero,
                Total = dto.Total,
                FechaEmision = DateTime.UtcNow,
                Comentario = dto.Comentario,
            };

            // 2. Llamada al Repo
            var resultado = await _comprobanteRepo.AddAsync(nuevaNota);

            // 3. Respuesta 201 Created
            return CreatedAtAction(
                nameof(GetComprobanteById),
                new { id = resultado.IdComprobante },
                resultado
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return StatusCode(
                500,
                new { mensaje = "Error al crear Nota de Débito", error = ex.Message }
            );
        }
    }

    [HttpPost("crear-orden")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearOrdenPago([FromBody] CreateOrdenPagoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // 1. Mapeo Manual DTO -> Dominio
            // (Nota: Podrías usar AutoMapper, pero hacerlo manual aquí es muy claro)
            var nuevaOrden = new Dom.OrdenPago
            {
                // IdProveedor = dto.IdProveedor,
                IdProveedor = dto.IdProveedor,
                Enviada = false, // Nace como borrador
                FechaPago = null, // Aún no se paga

                // 2. Lógica de Negocio: El total lo calcula el servidor, no el cliente.
                MontoTotal = dto.Detalles.Sum(d => d.MontoPagar),

                // 3. Mapeo de Detalles
                Detalles = dto
                    .Detalles.Select(d => new Dom.PagoDetalle
                    {
                        // IdFactura = d.IdFactura,
                        IdFactura = d.IdFactura,
                        MontoAplicado = d.MontoPagar,
                    })
                    .ToList(),
            };

            // 4. Guardar
            await _pagoRepo.CreateAsync(nuevaOrden);

            return CreatedAtAction(
                nameof(GetPagoById),
                new { id = nuevaOrden.IdOrdenPago },
                new { id = nuevaOrden.IdOrdenPago, total = nuevaOrden.MontoTotal }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/orden-pago/por-factura/5
    [HttpGet("pagos-por-factura/{idFactura}")]
    public async Task<IActionResult> GetPagosDeFactura(int idFactura)
    {
        if (idFactura <= 0)
            return BadRequest("El ID de la factura debe ser mayor a 0.");

        try
        {
            // Llamamos al método del repositorio
            var pagos = await _pagoRepo.GetPagosPorFacturaIdAsync(idFactura);

            return Ok(pagos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("comprobantes-por-factura/{idFactura}")]
    public async Task<IActionResult> GetComprobantesPorFactura(int idFactura)
    {
        // Validar que el ID sea lógico
        if (idFactura <= 0)
            return BadRequest("El ID de factura no es válido.");

        var comprobantes = await _comprobanteRepo.GetByFacturaIdAsync(idFactura);

        // Opcional: Si no hay nada, puedes devolver 200 con lista vacía (recomendado)
        // o 404 Not Found.
        return Ok(comprobantes);
    }

    [HttpPut("actualizar-orden")]
    public async Task<IActionResult> UpdateOrdenPago([FromBody] UpdateOrdenPagoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // PASO 1: Obtener la orden actual usando el REPO
            // Asumimos que tu IOrdenPagoRepository tiene un GetByIdAsync
            var ordenOriginal = await _pagoRepo.GetByIdWithDetallesAsync(dto.IdOrdenPago);

            if (ordenOriginal == null)
                return NotFound($"No se encontró la orden de pago con ID {dto.IdOrdenPago}");

            // REGLA: No editar si ya fue enviada
            if (ordenOriginal.Enviada)
            {
                return BadRequest(
                    new { error = "No se puede modificar una orden que ya fue enviada o pagada." }
                );
            }

            // PASO 2: Validar Facturas usando IFacturaRepository
            // Como no podemos hacer un query complejo con LINQ al contexto, iteramos.
            var facturasInvalidas = new List<string>();

            foreach (var detalle in dto.Detalles)
            {
                // Usamos el método de tu interfaz
                var factura = await _facturaRepo.GetByIdAsync(detalle.IdFactura);

                // Validación A: ¿Existe la factura?
                if (factura == null)
                {
                    facturasInvalidas.Add($"ID {detalle.IdFactura} (No existe)");
                    continue;
                }
                if (factura.Proveedor == null)
                {
                    facturasInvalidas.Add($"ID {detalle.IdFactura} (No tiene Proveedor asignado)");
                    continue;
                }

                // Validación B: ¿Es del mismo proveedor?
                if (factura.Proveedor.IdProveedor != ordenOriginal.IdProveedor)
                {
                    facturasInvalidas.Add(
                        $"Factura {factura.Numero} (Pertenece al Proveedor {factura.Proveedor.IdProveedor}, no al {ordenOriginal.IdProveedor})"
                    );
                }
            }

            // Si encontramos errores, devolvemos BadRequest
            if (facturasInvalidas.Any())
            {
                return BadRequest(
                    new { mensaje = "Error de Validación de Facturas", errores = facturasInvalidas }
                );
            }

            // PASO 3: Construcción del Objeto de Dominio
            var ordenParaActualizar = new Dom.OrdenPago
            {
                IdOrdenPago = dto.IdOrdenPago,
                IdProveedor = ordenOriginal.IdProveedor,
                MontoTotal = dto.Detalles.Sum(d => d.MontoPagar),
                Enviada = ordenOriginal.Enviada,
                FechaPago = ordenOriginal.FechaPago,

                // Mapeamos Detalles (Solo IDs y Montos)
                Detalles = dto
                    .Detalles.Select(d => new Dom.PagoDetalle
                    {
                        IdFactura = d.IdFactura,
                        MontoAplicado = d.MontoPagar,
                        // Factura = null (No asignamos el objeto para evitar conflictos)
                    })
                    .ToList(),
            };

            // PASO 4: Llamada al Repo para actualizar
            await _pagoRepo.UpdateAsync(ordenParaActualizar);

            return Ok(
                new
                {
                    mensaje = "Orden actualizada correctamente",
                    id = ordenParaActualizar.IdOrdenPago,
                }
            );
        }
        catch (KeyNotFoundException)
        {
            return NotFound("La orden no existe.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public record CreateNotaDto
    {
        // Validaciones básicas
        [System.ComponentModel.DataAnnotations.Required]
        public short IdProveedor { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public short IdCondicionPago { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public short IdMotivo { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public int IdFacturaReferencia { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string Numero { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        public decimal Total { get; set; }

        public string? Comentario { get; set; }
    }

    public record CreateOrdenPagoDto
    {
        [Required(ErrorMessage = "El proveedor es obligatorio.")]
        public short IdProveedor { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos una factura para pagar.")]
        public List<CreatePagoDetalleDto> Detalles { get; set; } = new();
    }

    // DTO Secundario (Filas/Detalles)
    public record CreatePagoDetalleDto
    {
        [Required]
        public int IdFactura { get; set; }

        [Required]
        [Range(0.01, 999999999, ErrorMessage = "El monto debe ser mayor a 0.")]
        public decimal MontoPagar { get; set; }
    }

    public record UpdateOrdenPagoDto
    {
        [Required]
        public int IdOrdenPago { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "La orden debe tener al menos una factura.")]
        public List<UpdatePagoDetalleDto> Detalles { get; set; } = new();
    }

    public record UpdatePagoDetalleDto
    {
        [Required]
        public int IdFactura { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal MontoPagar { get; set; }
    }
}

