// Repositories/ProveedorRepository.cs
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using src.Models;
using src.Models.CodeFirst;
using src.Repositories.Interfaces;


namespace src.Repositories.Implementations;

public class ProveedorRepository : IProveedorRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProveedorRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Proveedor?> GetProvByIdAsync(int id)
    {
        var proveedorEF = await _context.Proveedores
            .Include(p => p.id_condicion_pago_habitualNavigation)
            .Include(p => p.id_domicilioNavigation)
                .ThenInclude(d => d.id_provinciaNavigation)
            .FirstOrDefaultAsync(p => p.id_proveedor == id);

        if (proveedorEF == null) return null;

        var proveedor = _mapper.Map<Proveedor>(proveedorEF);

        // Lógica manual para la condición de pago
        if (proveedorEF.id_condicion_pago_habitualNavigation != null)
        {
            proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
        }

        return proveedor;
    }

    public async Task<List<Proveedor>> GetAllProvAsync()
    {
        var query = _context.Proveedores
            .Include(p => p.id_condicion_pago_habitualNavigation)
            .Include(p => p.id_domicilioNavigation)
                .ThenInclude(d => d.id_provinciaNavigation)
            .AsQueryable();

        query = query.Where(p => p.activo);

        var proveedoresEF = await query.ToListAsync();
        var proveedores = new List<Proveedor>();

        foreach (var proveedorEF in proveedoresEF)
        {
            var proveedor = _mapper.Map<Proveedor>(proveedorEF);

            if (proveedorEF.id_condicion_pago_habitualNavigation != null)
            {
                proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
            }

            proveedores.Add(proveedor);
        }

        return proveedores;
    }

    public async Task<Proveedor> CreateAsync(Proveedor proveedor)
    {

        if (proveedor.condicion != null)
        {
            await GuardarCondicionPagoAsync(proveedor.condicion);
        }

        if (proveedor.direccion != null)
        {
            await GuardarDireccionAsync(proveedor.direccion);
        }

        var proveedorEF = new proveedor
        {
            cuit = proveedor.cuit,
            razon_social = proveedor.razonSocial,
            telefono = proveedor.telefono,
            correo = proveedor.correo,
            persona_responsable = proveedor.personaResponsable,
            saldo = proveedor.saldo,
            activo = true,
            id_condicion_pago_habitual = proveedor.condicion?.id ?? 0, // ← Solo el ID
            id_domicilio = proveedor.direccion?.id ?? 0 // ← Solo el ID
        };

        _context.Proveedores.Add(proveedorEF);
        await _context.SaveChangesAsync();
        return await GetProvByIdAsync(proveedorEF.id_proveedor);
    }



    public async Task<Proveedor> UpdateAsync(Proveedor proveedor)
    {
        var proveedorEF = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.id_proveedor == proveedor.id);

        if (proveedorEF == null)
            throw new ArgumentException($"Proveedor con ID {proveedor.id} no encontrado");

        short? nuevaCondicionPagoId = null;
        if (proveedor.condicion != null)
        {
            await GuardarCondicionPagoAsync(proveedor.condicion);
            nuevaCondicionPagoId = proveedor.condicion.id;
        }

        short? nuevaDireccionId = null;
        if (proveedor.direccion != null)
        {
            // Crear nueva dirección independientemente de si ya existe una igual
            await GuardarDireccionAsync(proveedor.direccion);
            nuevaDireccionId = proveedor.direccion.id;
        }

        // 6. Actualizar propiedades del proveedor
        proveedorEF.cuit = proveedor.cuit;
        proveedorEF.razon_social = proveedor.razonSocial;
        proveedorEF.telefono = proveedor.telefono;
        proveedorEF.correo = proveedor.correo;
        proveedorEF.persona_responsable = proveedor.personaResponsable;
        proveedorEF.saldo = proveedor.saldo;
        proveedorEF.activo = proveedor.activo;


        if (nuevaCondicionPagoId.HasValue)
            proveedorEF.id_condicion_pago_habitual = nuevaCondicionPagoId.Value;

        if (nuevaDireccionId.HasValue)
            proveedorEF.id_domicilio = nuevaDireccionId.Value;

        await _context.SaveChangesAsync();


        return await GetProvByIdAsync(proveedor.id);
    }



    public async Task<bool> DeleteAsync(int id)
    {

        var proveedorEF = await _context.Proveedores
            .FirstOrDefaultAsync(p => p.id_proveedor == id);
        if (proveedorEF == null)
            return false;
        proveedorEF.activo = false;
        await _context.SaveChangesAsync();
        return true;
    }



    // Métodos privados para la lógica manual de mapeo

    private async Task<CondicionDePago> MapCondicionPagoAsync(short condicionPagoId)
    {
        var contado = await _context.Contado
            .FirstOrDefaultAsync(c => c.id_condicion_pago == condicionPagoId);

        if (contado != null)
        {
            var contadoMapeado = _mapper.Map<Contado>(contado);
            contadoMapeado.Tipo = "Contado";
            return contadoMapeado;
        }

        // Verificar si es Cuota
        var cuota = await _context.Cuota
            .FirstOrDefaultAsync(c => c.id_condicion_pago == condicionPagoId);

        if (cuota != null)
        {
            var cuotaMapeada = _mapper.Map<Cuota>(cuota);
            cuotaMapeada.Tipo = "Cuota";
            return cuotaMapeada;
        }

        return null;
    }

    /*private async Task GuardarCondicionPagoAsync(CondicionDePago condicion)
    {

        var condicionBase = new condicion_pago
        {
            dias_pago = condicion.dias_pago
        };

        _context.condicion_pago.Add(condicionBase);
        await _context.SaveChangesAsync(); // Para obtener el ID generado


        if (condicion is Cuota cuota)
        {
            var cuotaEF = _mapper.Map<cuota>(cuota);
            _context.Cuota.Add(cuotaEF);
        }
        else if (condicion is Contado contado)
        {
            var contadoEF = _mapper.Map<contado>(contado);
            _context.Contado.Add(contadoEF);
        }
        await _context.SaveChangesAsync();
        condicion.id = condicionBase.id_condicion_pago; // Actualizar el ID en el objeto dominio
    } */


    private async Task GuardarCondicionPagoAsync(CondicionDePago condicion)
    {
        if (condicion is Cuota cuota)
        {
            var cuotaEF = new cuota
            {
                dias_pago = cuota.dias_pago,
                cuotas = cuota.numeroCuotas,
                interes_porcentual = (decimal)cuota.interes_porcentual
            };

            _context.Cuota.Add(cuotaEF);
            await _context.SaveChangesAsync();
            condicion.id = cuotaEF.id_condicion_pago; // ← ACTUALIZAR ID
        }
        else if (condicion is Contado contado)
        {
            var contadoEF = new contado
            {
                dias_pago = contado.dias_pago
            };

            _context.Contado.Add(contadoEF);
            await _context.SaveChangesAsync();
            condicion.id = contadoEF.id_condicion_pago; // ← ACTUALIZAR ID
        }
    }

    private async Task GuardarDireccionAsync(Direccion direccion)
    {
        var direccionEF = new domicilio
        {
            calle = direccion.calle,
            numero = direccion.numero,
            piso = direccion.piso,
            comentario = direccion.comentario,
            id_provincia = direccion.id_provincia
        };

        _context.Domicilios.Add(direccionEF);
        await _context.SaveChangesAsync();
        direccion.id = direccionEF.id_domicilio; // ← ACTUALIZAR ID
    }


   
    public async Task<List<Provincia>> GetAllProvinciasAsync()
    {
        var provinciasEF = await _context.Provincias.ToListAsync();
        var provinciasMapeadas = _mapper.Map<List<Provincia>>(provinciasEF);
        return provinciasMapeadas;
    }
   




}

















/*
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
            _context.Proveedores.AddAsync(proveedor);
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
            return await _context.proveedor.FindAsync(id);
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



*/



/* METODOS QUE SUGIERE CHATGPT PARA UN FUTURO 

 public async Task<bool> ToggleActivoAsync(int id)
        {
            var proveedorEF = await _context.proveedores.FindAsync(id);
            if (proveedorEF == null) return false;

            proveedorEF.activo = !proveedorEF.activo;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetSaldoTotalAsync()
        {
            return await _context.proveedores
                .Where(p => p.activo)
                .SumAsync(p => p.saldo);
        }

        public async Task<bool> ExistsByCuitAsync(string cuit, int? excludeId = null)
        {
            var query = _context.proveedores.Where(p => p.cuit == cuit);
            
            if (excludeId.HasValue)
                query = query.Where(p => p.id_proveedor != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<List<Proveedor>> GetProveedoresConSaldoPositivoAsync()
        {
            var proveedoresEF = await _context.proveedores
                .Include(p => p.id_condicion_pago_habitualNavigation)
                .Include(p => p.id_domicilioNavigation)
                    .ThenInclude(d => d.id_provinciaNavigation)
                .Where(p => p.activo && p.saldo > 0)
                .ToListAsync();

            var proveedores = new List<Proveedor>();
            foreach (var proveedorEF in proveedoresEF)
            {
                var proveedor = _mapper.Map<Proveedor>(proveedorEF);
                proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
                proveedores.Add(proveedor);
            }

            return proveedores;
        }

        public async Task<List<Proveedor>> GetProveedoresConDireccionCompletaAsync()
        {
            var proveedoresEF = await _context.proveedores
                .Include(p => p.id_condicion_pago_habitualNavigation)
                .Include(p => p.id_domicilioNavigation)
                    .ThenInclude(d => d.id_provinciaNavigation)
                .Where(p => p.activo && p.id_domicilioNavigation != null)
                .ToListAsync();

            var proveedores = new List<Proveedor>();
            foreach (var proveedorEF in proveedoresEF)
            {
                var proveedor = _mapper.Map<Proveedor>(proveedorEF);
                proveedor.condicion = await MapCondicionPagoAsync(proveedorEF.id_condicion_pago_habitual);
                proveedores.Add(proveedor);
            }

            return proveedores;
        }*/