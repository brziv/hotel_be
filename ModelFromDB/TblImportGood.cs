using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_ImportGoods")]
public partial class TblImportGood
{
    [Key]
    [Column("ig_ImportID")]
    public Guid IgImportId { get; set; }

    [Column("ig_SumPrice", TypeName = "decimal(10, 2)")]
    public decimal IgSumPrice { get; set; }

    [Column("ig_Currency")]
    [StringLength(30)]
    public string IgCurrency { get; set; } = null!;

    [Column("ig_ImportDate", TypeName = "datetime")]
    public DateTime? IgImportDate { get; set; }

    [Column("ig_Supplier")]
    [StringLength(200)]
    public string? IgSupplier { get; set; }

    [InverseProperty("IgdImport")]
    public virtual ICollection<TblImportGoodsDetail> TblImportGoodsDetails { get; set; } = new List<TblImportGoodsDetail>();
}
