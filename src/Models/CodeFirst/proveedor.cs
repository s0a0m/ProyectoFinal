using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

public partial class Proveedor
{
    [Column("id_proveedor")]
    public short IdProveedor { get; set; }
    [Column("cuit")]
    public string Cuit { get; set; }

    [Column("razon_social")]
    public string RazonSocial { get; set; }
    [Column("id_condicion_pago_habitual")]
    public short IdCondicionPagoHabitual { get; set; }

    [Column("telefono")]
    public string Telefono { get; set; }

    [Column("correo")]
    public string Correo { get; set; }

    [Column("persona_responsable")]
    public string PersonaResponsable { get; set; }
    [Column("saldo", TypeName = "decimal(11, 2)")]
    public decimal Saldo { get; set; }
    [Column("id_domicilio")]
    public short IdDomicilio { get; set; }
    [Column("activo")]
    public bool Activo { get; set; }

    public virtual CondicionDePago IdCondicionPagoHabitualNavigation { get; set; }

    public virtual Domicilio IdDomicilioNavigation { get; set; }
}
