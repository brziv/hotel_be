using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Guests")]
[Index("GEmail", Name = "UQ__tbl_Gues__317ADEF737721475", IsUnique = true)]
public partial class TblGuest
{
    [Key]
    [Column("g_GuestID")]
    public Guid GGuestId { get; set; }

    [Column("g_FirstName")]
    [StringLength(50)]
    public string GFirstName { get; set; } = null!;

    [Column("g_LastName")]
    [StringLength(50)]
    public string GLastName { get; set; } = null!;

    [Column("g_Email")]
    [StringLength(100)]
    public string? GEmail { get; set; }

    [Column("g_PhoneNumber")]
    [StringLength(15)]
    public string GPhoneNumber { get; set; } = null!;

    [InverseProperty("BGuest")]
    public virtual ICollection<TblBooking> TblBookings { get; set; } = new List<TblBooking>();
}
