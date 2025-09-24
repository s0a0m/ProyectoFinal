using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class proveedor
{
    public short id_proveedor { get; set; }

    public string cuit { get; set; }

    public string razon_social { get; set; }

    public string domicilio { get; set; }

    public short id_condicion_pago_habitual { get; set; }

    public string telefono { get; set; }

    public string correo { get; set; }

    public string persona_responsable { get; set; }

    public string saldo { get; set; }

    public short id_direccion { get; set; }

    public bool activo { get; set; }

    public virtual condicion_pago id_condicion_pago_habitualNavigation { get; set; }

    public virtual direccion id_direccionNavigation { get; set; }
}
