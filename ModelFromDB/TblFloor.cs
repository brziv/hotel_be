using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblFloor
{
    public Guid FFloorId { get; set; }

    public string FFloor { get; set; } = null!;

    public virtual ICollection<TblRoom> TblRooms { get; set; } = new List<TblRoom>();
}
