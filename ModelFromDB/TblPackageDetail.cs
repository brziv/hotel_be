using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblPackageDetail
{
    public Guid PdDetailId { get; set; }

    public Guid PdPackageId { get; set; }

    public Guid PdProductId { get; set; }

    public int PdQuantity { get; set; }

    public virtual TblServicePackage PdPackage { get; set; } = null!;

    public virtual TblProduct PdProduct { get; set; } = null!;
}
