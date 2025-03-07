using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Goods")]
public partial class TblGood
{
    [Key]
    [Column("g_GoodsID")]
    public Guid GGoodsId { get; set; }

    [Column("g_GoodsName")]
    [StringLength(255)]
    public string GGoodsName { get; set; } = null!;

    [Column("g_Category")]
    [StringLength(100)]
    public string? GCategory { get; set; }

    [Column("g_Quantity")]
    public int? GQuantity { get; set; }

    [Column("g_Unit")]
    [StringLength(30)]
    public string? GUnit { get; set; }

    [Column("g_CostPrice", TypeName = "decimal(10, 2)")]
    public decimal GCostPrice { get; set; }

    [Column("g_SellingPrice", TypeName = "decimal(10, 2)")]
    public decimal GSellingPrice { get; set; }

    [Column("g_Currency")]
    [StringLength(30)]
    public string GCurrency { get; set; } = null!;

    [InverseProperty("IgdGoods")]
    public virtual ICollection<TblImportGoodsDetail> TblImportGoodsDetails { get; set; } = new List<TblImportGoodsDetail>();

    [InverseProperty("SgGoods")]
    public virtual ICollection<TblServiceGood> TblServiceGoods { get; set; } = new List<TblServiceGood>();
}
