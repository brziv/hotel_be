using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblImportGoodsDetail
{
    public Guid IgdId { get; set; }

    public Guid IgdImportId { get; set; }

    public Guid IgdGoodsId { get; set; }

    public int IgdQuantity { get; set; }

    public decimal IgdCostPrice { get; set; }

    public virtual TblProduct? IgdGoods { get; set; }

    public virtual TblImportGood? IgdImport { get; set; }
}
