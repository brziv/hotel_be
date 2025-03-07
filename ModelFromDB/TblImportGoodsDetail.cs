using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_ImportGoodsDetails")]
public partial class TblImportGoodsDetail
{
    [Key]
    [Column("igd_ID")]
    public Guid IgdId { get; set; }

    [Column("igd_ImportID")]
    public Guid IgdImportId { get; set; }

    [Column("igd_GoodsID")]
    public Guid IgdGoodsId { get; set; }

    [Column("igd_Quantity")]
    public int IgdQuantity { get; set; }

    [Column("igd_CostPrice", TypeName = "decimal(10, 2)")]
    public decimal IgdCostPrice { get; set; }

    [ForeignKey("IgdGoodsId")]
    [InverseProperty("TblImportGoodsDetails")]
    public virtual TblGood IgdGoods { get; set; } = null!;

    [ForeignKey("IgdImportId")]
    [InverseProperty("TblImportGoodsDetails")]
    public virtual TblImportGood IgdImport { get; set; } = null!;
}
