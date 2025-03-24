using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblServicePackage
{
    public Guid SpPackageId { get; set; }

    public string SpPackageName { get; set; } = null!;

    public decimal SServiceCostPrice { get; set; }

    public decimal SServiceSellPrice { get; set; }

    public virtual ICollection<TblBookingService> TblBookingServices { get; set; } = new List<TblBookingService>();

    public virtual ICollection<TblPackageDetail> TblPackageDetails { get; set; } = new List<TblPackageDetail>();
}
