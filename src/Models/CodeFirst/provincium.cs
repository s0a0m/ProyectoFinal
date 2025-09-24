using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class provincium
{
    public short id_provincia { get; set; }

    public string provincia { get; set; }

    public virtual ICollection<direccion> direccions { get; set; } = new List<direccion>();
}
