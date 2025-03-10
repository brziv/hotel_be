using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Bookings")]
public partial class TblBooking
{
    [Key]
    [Column("b_BookingID")]
    public Guid BBookingId { get; set; }

    [Column("b_GuestID")]
    public Guid BGuestId { get; set; }

    [Column("b_BookingStatus")]
    [StringLength(20)]
    public string BBookingStatus { get; set; } = null!;

    [Column("b_TotalMoney", TypeName = "decimal(10, 2)")]
    public decimal? BTotalMoney { get; set; }

    [Column("b_Deposit", TypeName = "decimal(10, 2)")]
    public decimal? BDeposit { get; set; }

    [Column("b_CreatedAt", TypeName = "datetime")]
    public DateTime? BCreatedAt { get; set; }

    [ForeignKey("BGuestId")]
    [InverseProperty("TblBookings")]
    public virtual TblGuest BGuest { get; set; } = null!;

    [InverseProperty("BrBooking")]
    public virtual ICollection<TblBookingRoom> TblBookingRooms { get; set; } = new List<TblBookingRoom>();

    [InverseProperty("BsBooking")]
    public virtual ICollection<TblBookingService> TblBookingServices { get; set; } = new List<TblBookingService>();

    [InverseProperty("PBooking")]
    public virtual ICollection<TblPayment> TblPayments { get; set; } = new List<TblPayment>();
}
