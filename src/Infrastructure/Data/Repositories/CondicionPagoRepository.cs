using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;
using src.Repositories.Interfaces;
using src.Models.Domain;
using src.Models.Common;

namespace src.Repositories.Implementations
{
    public class CondicionPagoRepository : ICondicionPagoRepository
    {
        private readonly EF.AppDbContext _context;

        public CondicionPagoRepository(EF.AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dom.CondicionDePago>> GetAllAsync()
        {
            var condicionesEF = await _context.Condicion_pagos
                .AsNoTracking()
                .ToListAsync();

            return DominioMapper.Map(condicionesEF);
        }

        public async Task<Dom.CondicionDePago?> GetByIdAsync(int id)
        {
            var condicionEF = await _context.Condicion_pagos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCondicionPago == id);

            if (condicionEF == null) return null;

            return DominioMapper.Map(condicionEF);
        }


// ... imports

    public async Task<Dom.CondicionDePago> BuscarOCrearAsync(Dom.CondicionDePago condicion)
    {
        if (condicion is Dom.Contado contadoDom)
        {
            // 1. Buscar si existe Contado con esos días
            var existente = await _context.Condicion_pagos
                .OfType<EF.Contado>() // Filtra solo tablas de tipo Contado
                .FirstOrDefaultAsync(c => c.DiasPago == contadoDom.DiasPago);

            if (existente != null) return DominioMapper.Map(existente);

            // 2. Si no existe, crear
            var nuevo = new EF.Contado { DiasPago = contadoDom.DiasPago };
            _context.Condicion_pagos.Add(nuevo);
            await _context.SaveChangesAsync();
            return DominioMapper.Map(nuevo);
        }
        else if (condicion is Dom.Cuota cuotaDom)
        {
            // 1. Buscar si existe Cuota con esos parámetros exactos
            var existente = await _context.Condicion_pagos
                .OfType<EF.Cuota>()
                .FirstOrDefaultAsync(c => c.DiasPago == cuotaDom.DiasPago && 
                                        c.Cuotas == cuotaDom.Cuotas && 
                                        c.InteresPorcentual == cuotaDom.InteresPorcentual);

            if (existente != null) return DominioMapper.Map(existente);

            // 2. Si no existe, crear
            var nuevo = new EF.Cuota 
            { 
                DiasPago = cuotaDom.DiasPago,
                Cuotas = cuotaDom.Cuotas,
                InteresPorcentual = cuotaDom.InteresPorcentual
            };
            _context.Condicion_pagos.Add(nuevo);
            await _context.SaveChangesAsync();
            return DominioMapper.Map(nuevo);
        }

        throw new ArgumentException("Tipo de condición de pago no soportado");
    }



    }
}