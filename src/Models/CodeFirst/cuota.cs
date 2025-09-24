using System;
using System.Collections.Generic;

namespace src.Models.CodeFirst;

public partial class cuota
{
    public short id_condicion_pago { get; set; }

    public short dias_pago { get; set; }

    public short cuotas { get; set; }

    public short interes_porcentual { get; set; }
}
