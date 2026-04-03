using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace src.Models.CodeFirst;

[Table("deposito")]
public class Deposito
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_deposito")]
    public int IdDeposito { get; set; }

    [Required]
    [StringLength(100)]
    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    // Asumiendo que Direccion es otra entidad. Si es un Value Object, deberías usar [Owned]
    [Column("id_direccion")]
    public short IdDireccion { get; set; }
    
    [ForeignKey(nameof(IdDireccion))]
    public Domicilio Direccion { get; set; }  = new Domicilio();

    [Column("activo")]
    public bool Activo { get; set; }

    // Propiedad de navegación inversa
    public ICollection<Estante> Estantes { get; set; } = new List<Estante>();
}