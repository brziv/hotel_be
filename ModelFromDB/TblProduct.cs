using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblProduct
{
    public Guid PProductId { get; set; }

    public string PProductName { get; set; } = null!;

    public string? PCategory { get; set; }

    public int? PQuantity { get; set; }

    public string? PUnit { get; set; }

    public decimal PCostPrice { get; set; }

    public decimal PSellingPrice { get; set; }

    public string PCurrency { get; set; } = null!;

    public bool? PIsService { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<TblImportGoodsDetail> TblImportGoodsDetails { get; set; } = new List<TblImportGoodsDetail>();

    public virtual ICollection<TblPackageDetail> TblPackageDetails { get; set; } = new List<TblPackageDetail>();
}
