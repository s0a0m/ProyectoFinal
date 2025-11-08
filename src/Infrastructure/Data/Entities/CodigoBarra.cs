using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace src.Models.CodeFirst;

[Table("codigo_barra")]
public class CodigoBarra
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id_codigo_barra")]
    public short IdCodigoBarra { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("codigo_barra")]
    public string Codigo { get; set; }

    public Producto Producto { get; set; }
}
