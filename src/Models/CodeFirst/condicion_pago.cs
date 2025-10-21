using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

public abstract class condicion_pago
{
    [Key]
    [Column("id_condicion_pago")]
    public short id_condicion_pago { get; set; }

    [Column("dias_pago")]
    public short dias_pago { get; set; }
}
