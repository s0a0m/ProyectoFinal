using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;


[Table("cuotas")]
public class Cuota : CondicionDePago
{
    [Column("cuotas")]
    public short Cuotas { get; set; }
    [Column("interes_porcentual", TypeName = "decimal(6, 2)")]
    public decimal InteresPorcentual { get; set; }
}
