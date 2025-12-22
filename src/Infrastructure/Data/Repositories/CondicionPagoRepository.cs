using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Repositories.Interfaces;
using Dom = src.Models.Domain;
using EF = src.Models.CodeFirst;
using src.Models.Mappers;
using src.Interfaces;
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
    }
}