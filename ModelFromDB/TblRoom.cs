using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblRoom
{
    public Guid RRoomId { get; set; }

    public string RRoomNumber { get; set; } = null!;

    public Guid RFloorId { get; set; }

    public string RRoomType { get; set; } = null!;

    public decimal RPricePerHour { get; set; }

    public string RStatus { get; set; } = null!;

    public virtual TblFloor RFloor { get; set; } = null!;

    public virtual ICollection<TblBookingRoom> TblBookingRooms { get; set; } = new List<TblBookingRoom>();
}
