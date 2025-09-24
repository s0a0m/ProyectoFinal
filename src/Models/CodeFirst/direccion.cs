using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class direccion
{
    public short id_direccion { get; set; }

    public short id_provincia { get; set; }

    public string calle { get; set; }

    public short numero { get; set; }

    public short? piso { get; set; }

    public string comentario { get; set; }

    public virtual provincium id_provinciaNavigation { get; set; }

    public virtual ICollection<proveedor> proveedors { get; set; } = new List<proveedor>();
}
