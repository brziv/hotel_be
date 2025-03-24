using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblBooking
{
    public Guid BBookingId { get; set; }

    public Guid BGuestId { get; set; }

    public string BBookingStatus { get; set; } = null!;

    public decimal? BTotalMoney { get; set; }

    public decimal? BDeposit { get; set; }

    public DateTime? BCreatedAt { get; set; }

    public virtual TblGuest BGuest { get; set; } = null!;

    public virtual ICollection<TblBookingRoom> TblBookingRooms { get; set; } = new List<TblBookingRoom>();

    public virtual ICollection<TblBookingService> TblBookingServices { get; set; } = new List<TblBookingService>();

    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
