using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("domicilio")]
public partial class Domicilio
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_domicilio")]
    public short IdDomicilio { get; set; }

    [Column("id_provincia")]
    public short IdProvincia { get; set; }

    [Column("calle")]
    public string Calle { get; set; }

    [Column("numero")]
    public short Numero { get; set; }

    [Column("piso")]
    public short? Piso { get; set; }

    [Column("comentario")]
    public string? Comentario { get; set; }

    public virtual Provincia IdProvinciaNavigation { get; set; }

    public virtual ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
}
