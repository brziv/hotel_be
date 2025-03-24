using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblPayment
{
    public Guid PPaymentId { get; set; }

    public Guid PBookingId { get; set; }

    public decimal PAmountPaid { get; set; }

    public string PPaymentMethod { get; set; } = null!;

    public DateTime? PPaymentDate { get; set; }

    public virtual TblBooking PBooking { get; set; } = null!;
}
