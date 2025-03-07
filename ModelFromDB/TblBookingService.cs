using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_BookingServices")]
public partial class TblBookingService
{
    [Key]
    [Column("bs_BookingServicesID")]
    public Guid BsBookingServicesId { get; set; }

    [Column("bs_BookingID")]
    public Guid BsBookingId { get; set; }

    [Column("bs_ServiceID")]
    public Guid BsServiceId { get; set; }

    [Column("bs_Quantity")]
    public int BsQuantity { get; set; }

    [Column("bs_CreatedAt", TypeName = "datetime")]
    public DateTime? BsCreatedAt { get; set; }

    [ForeignKey("BsBookingId")]
    [InverseProperty("TblBookingServices")]
    public virtual TblBooking BsBooking { get; set; } = null!;

    [ForeignKey("BsServiceId")]
    [InverseProperty("TblBookingServices")]
    public virtual TblService BsService { get; set; } = null!;
}
