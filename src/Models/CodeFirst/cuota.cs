using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

public partial class cuota
{
    public short id_condicion_pago { get; set; }

    public short dias_pago { get; set; }

    public short cuotas { get; set; }
    [Column(TypeName = "decimal(6, 2)")]
    public decimal interes_porcentual { get; set; }
}
