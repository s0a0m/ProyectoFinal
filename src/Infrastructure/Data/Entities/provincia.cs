using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("provincia")]
public partial class Provincia
{
    [Column("id_provincia")]
    public short IdProvincia { get; set; }
    [Column("nombre")]
    public string Nombre { get; set; }

    public virtual ICollection<Domicilio> Domicilios { get; set; } = new List<Domicilio>();
}
