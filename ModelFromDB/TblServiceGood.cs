using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_ServiceGoods")]
public partial class TblServiceGood
{
    [Key]
    [Column("sg_ServiceGoodsID")]
    public Guid SgServiceGoodsId { get; set; }

    [Column("sg_ServiceID")]
    public Guid SgServiceId { get; set; }

    [Column("sg_GoodsID")]
    public Guid SgGoodsId { get; set; }

    [Column("sg_Quantity")]
    public int SgQuantity { get; set; }

    [ForeignKey("SgGoodsId")]
    [InverseProperty("TblServiceGoods")]
    public virtual TblGood SgGoods { get; set; } = null!;

    [ForeignKey("SgServiceId")]
    [InverseProperty("TblServiceGoods")]
    public virtual TblService SgService { get; set; } = null!;
}
