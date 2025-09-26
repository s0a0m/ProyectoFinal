using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("provincia")]
public partial class provincia
{
    public short id_provincia { get; set; }
    public string nombre { get; set; }

    public virtual ICollection<domicilio> domicilios { get; set; } = new List<domicilio>();
}
