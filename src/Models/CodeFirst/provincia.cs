using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class provincia
{
    public short id_provincia { get; set; }
    public string nombre { get; set; }

    public virtual ICollection<domicilio> domicilios { get; set; } = new List<domicilio>();
}
