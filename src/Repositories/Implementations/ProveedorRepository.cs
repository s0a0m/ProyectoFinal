using src.Models;
using src.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using src.Models.CodeFirst;

namespace src.Repositories.Implementations
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly AppDbContext _context;

        public ProveedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(proveedor proveedor)
        {
            _context.proveedors.Add(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var proveedor = await _context.proveedors.FindAsync(id);
            if (proveedor != null)
            {
                _context.proveedors.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<proveedor>> GetAll()
        {
            return await _context.proveedors.ToListAsync();
        }

        public async Task<proveedor?> GetById(int id)
        {
            return await _context.proveedors.FindAsync(id);
        }

        public async Task Update(proveedor proveedor)
        {
            _context.Entry(proveedor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        // private Proveedor MapToDom(proveedor p)
        // {
        //     return new Proveedor
        //     {
        //         Id = p.id_proveedor,
        //         Cuit = p.cuit,
        //         Domicilio = new Direccion { Calle = p.domicilio },
        //         Telefono = p.telefono,
        //         Correo = p.correo,
        //         PersonaResponsable = p.persona_responsable,
        //         Saldo = decimal.Parse(p.saldo),
        //         Activo = p.activo
        //     };
        // }
        // private proveedor MapToEF(Proveedor p)
        // {
        //     return new proveedor
        //     {
        //         id_proveedor = p.Id,
        //         cuit = p.Cuit,
        //         domicilio = p.Domicilio.Calle,
        //         telefono = p.Telefono,
        //         correo = p.Correo,
        //         persona_responsable = p.PersonaResponsable,
        //         saldo = p.Saldo.ToString(),
        //         activo = p.Activo
        //     };
        // }
    }
}