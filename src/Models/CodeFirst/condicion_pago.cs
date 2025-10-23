using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("condicion_pago")]
public abstract class CondicionDePago
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_condicion_pago")]
    public short IdCondicionPago { get; set; }

    [Column("dias_pago")]
    public short DiasPago { get; set; }
}
