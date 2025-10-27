using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("contado")]
public class Contado : CondicionDePago
{
    // No agrega campos nuevos (solo hereda)
}
