using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;


[Table("cuotas")]
public class cuota : condicion_pago
{
    [Column("cuotas")]
    public short cuotas { get; set; }
    [Column("interes_porcentual", TypeName = "decimal(6, 2)")]
    public decimal interes_porcentual { get; set; }
}
