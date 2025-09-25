using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class condicion_pago
{
    public short id_condicion_pago { get; set; }

    public short dias_pago { get; set; }

    public virtual ICollection<proveedor> proveedores { get; set; } = new List<proveedor>();
}
