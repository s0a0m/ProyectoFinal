using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("contado")]
public class contado : condicion_pago
{
    // No agrega campos nuevos (solo hereda)
}
