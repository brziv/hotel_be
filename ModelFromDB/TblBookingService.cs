using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblBookingService
{
    public Guid BsId { get; set; }

    public Guid BsBookingId { get; set; }

    public Guid BsServiceId { get; set; }

    public int BsQuantity { get; set; }

    public DateTime? BsCreatedAt { get; set; }

    public virtual TblBooking BsBooking { get; set; } = null!;

    public virtual TblServicePackage BsService { get; set; } = null!;
}
