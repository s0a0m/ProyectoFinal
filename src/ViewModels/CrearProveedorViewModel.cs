using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using src.Models.CodeFirst;
using Dom = src.Models.Domain;
namespace src.ViewModels;
    public class CrearProveedorViewModel
    {
        public CrearProveedorViewModel()
        {
        }

        [Required(ErrorMessage = "El CUIT es obligatorio.")]
        public string Cuit { get; set; }

        [Required(ErrorMessage = "La razon social es obligatoria.")]
        [StringLength(100)]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El numero de telefono es obligatorio.")]
        [StringLength(12)]
        public string Telefono { get; set; }


    [Required(ErrorMessage = "El Mail es obligatorio.")]
    [EmailAddress, StringLength(50)]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El nombre de la Persona responsable es obligatorio.")]
        [StringLength(80)]
        public string PersonaResponsable { get; set; }

        [Required(ErrorMessage = "El Saldo es obligatorio.")]
        [ Range(0, 9999999999.99)]
        public decimal Saldo { get; set; }


        [Required(ErrorMessage = "La condicion de pago es obligatoria.")]
        public CondicionDePagoViewModel CondicionPago { get; set; } = new();


        [Required(ErrorMessage = "La Direccion es obligatoria.")]
        public DireccionViewModel Direccion { get; set; } = new();

        public static Dom.Proveedor cargarProveedor(CrearProveedorViewModel p)
        {
            Dom.Proveedor pDom = new()
            {
                Activo = true,
                Cuit = p.Cuit,
                Telefono = p.Telefono,
                Correo = p.Correo,
                PersonaResponsable = p.PersonaResponsable,
                Saldo = p.Saldo,
                RazonSocial = p.RazonSocial
            };

            pDom.Direccion.Calle = p.Direccion.calle;
            pDom.Direccion.Comentario = p.Direccion.comentario;
            pDom.Direccion.Numero = p.Direccion.numero;
            pDom.Direccion.Piso = p.Direccion.piso;
           Dom.Provincia provinciaSeleccionada = p.Direccion.ListaProvincias.FirstOrDefault(prov => prov.IdProvincia == p.Direccion.provincia.Id_provincia);

            // 2. Asigna la instancia de objeto COMPLETA.
            pDom.Direccion.Prov = provinciaSeleccionada;
            if (p.CondicionPago.Tipo == "Cuota")
            {
                Dom.Cuota c = new()
                {
                    Cuotas = p.CondicionPago.NumeroCuotas,
                    DiasPago = p.CondicionPago.DiasPago,
                    InteresPorcentual = p.CondicionPago.InteresPorcentual

                };
                
                pDom.Condicion = c;

            }else
            {
                if(p.CondicionPago.Tipo == "Contado")
                {
                    Dom.Contado contado = new()
                    {
                        DiasPago = p.CondicionPago.DiasPago
                    };
                    pDom.Condicion = contado;
                }   
            }
            return pDom;

        } 

    }
