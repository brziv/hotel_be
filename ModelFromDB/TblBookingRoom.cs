using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_BookingRooms")]
public partial class TblBookingRoom
{
    [Key]
    [Column("br_BookingRoomsID")]
    public Guid BrBookingRoomsId { get; set; }

    [Column("br_BookingID")]
    public Guid BrBookingId { get; set; }

    [Column("br_RoomID")]
    public Guid BrRoomId { get; set; }

    [Column("b_CheckInDate", TypeName = "datetime")]
    public DateTime BCheckInDate { get; set; }

    [Column("b_CheckOutDate", TypeName = "datetime")]
    public DateTime BCheckOutDate { get; set; }

    [ForeignKey("BrBookingId")]
    [InverseProperty("TblBookingRooms")]
    public virtual TblBooking BrBooking { get; set; } = null!;

    [ForeignKey("BrRoomId")]
    [InverseProperty("TblBookingRooms")]
    public virtual TblRoom BrRoom { get; set; } = null!;
}
