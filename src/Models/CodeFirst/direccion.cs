using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class domicilio
{
    public short id_domicilio { get; set; }

    public short id_provincia { get; set; }

    public string calle { get; set; }

    public short numero { get; set; }

    public short? piso { get; set; }
    // cambiar para que sea nulleable
    public string comentario { get; set; }

    public virtual provincia id_provinciaNavigation { get; set; }

    public virtual ICollection<proveedor> proveedores { get; set; } = new List<proveedor>();
}
