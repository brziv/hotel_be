using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Services")]
public partial class TblService
{
    [Key]
    [Column("s_ServiceID")]
    public Guid SServiceId { get; set; }

    [Column("s_ServiceName")]
    [StringLength(100)]
    public string SServiceName { get; set; } = null!;

    [Column("s_ServiceCostPrice", TypeName = "decimal(10, 2)")]
    public decimal SServiceCostPrice { get; set; }

    [Column("s_ServiceSellPrice", TypeName = "decimal(10, 2)")]
    public decimal SServiceSellPrice { get; set; }

    [InverseProperty("BsService")]
    public virtual ICollection<TblBookingService> TblBookingServices { get; set; } = new List<TblBookingService>();

    [InverseProperty("SgService")]
    public virtual ICollection<TblServiceGood> TblServiceGoods { get; set; } = new List<TblServiceGood>();
}
