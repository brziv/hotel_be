using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Rooms")]
[Index("RRoomNumber", Name = "UQ__tbl_Room__64AD69DE794F86F1", IsUnique = true)]
public partial class TblRoom
{
    [Key]
    [Column("r_RoomID")]
    public Guid RRoomId { get; set; }

    [Column("r_RoomNumber")]
    [StringLength(10)]
    public string RRoomNumber { get; set; } = null!;

    [Column("r_FloorID")]
    public Guid RFloorId { get; set; }

    [Column("r_RoomType")]
    [StringLength(50)]
    public string RRoomType { get; set; } = null!;

    [Column("r_PricePerHour", TypeName = "decimal(10, 2)")]
    public decimal RPricePerHour { get; set; }

    [Column("r_Status")]
    [StringLength(20)]
    public string RStatus { get; set; } = null!;

    [ForeignKey("RFloorId")]
    [InverseProperty("TblRooms")]
    public virtual TblFloor RFloor { get; set; } = null!;

    [InverseProperty("BrRoom")]
    public virtual ICollection<TblBookingRoom> TblBookingRooms { get; set; } = new List<TblBookingRoom>();
}
