using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblImportGood
{
    public Guid IgImportId { get; set; }

    public decimal IgSumPrice { get; set; }

    public string IgCurrency { get; set; } = null!;

    public DateTime? IgImportDate { get; set; }

    public string? IgSupplier { get; set; }

    public virtual ICollection<TblImportGoodsDetail> TblImportGoodsDetails { get; set; } = new List<TblImportGoodsDetail>();
}
