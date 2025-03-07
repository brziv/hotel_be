using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Payments")]
public partial class TblPayment
{
    [Key]
    [Column("p_PaymentID")]
    public Guid PPaymentId { get; set; }

    [Column("p_BookingID")]
    public Guid PBookingId { get; set; }

    [Column("p_AmountPaid", TypeName = "decimal(10, 2)")]
    public decimal PAmountPaid { get; set; }

    [Column("p_PaymentMethod")]
    [StringLength(50)]
    public string PPaymentMethod { get; set; } = null!;

    [Column("p_PaymentDate", TypeName = "datetime")]
    public DateTime? PPaymentDate { get; set; }

    [ForeignKey("PBookingId")]
    [InverseProperty("TblPayments")]
    public virtual TblBooking PBooking { get; set; } = null!;
}
