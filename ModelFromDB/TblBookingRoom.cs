using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblBookingRoom
{
    public Guid BrId { get; set; }

    public Guid BrBookingId { get; set; }

    public Guid BrRoomId { get; set; }

    public DateTime BrCheckInDate { get; set; }

    public DateTime BrCheckOutDate { get; set; }

    public virtual TblBooking BrBooking { get; set; } = null!;

    public virtual TblRoom BrRoom { get; set; } = null!;
}
