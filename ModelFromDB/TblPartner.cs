using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Partner")]
[Index("PEmail", Name = "UQ__tbl_Part__1DFC0D6D78CD48CF", IsUnique = true)]
public partial class TblPartner
{
    [Key]
    [Column("p_PartnerID")]
    public Guid PPartnerId { get; set; }

    [Column("p_PartnerName")]
    [StringLength(255)]
    public string PPartnerName { get; set; } = null!;

    [Column("p_PartnerType")]
    [StringLength(100)]
    public string? PPartnerType { get; set; }

    [Column("p_PhoneNumber")]
    [StringLength(15)]
    public string PPhoneNumber { get; set; } = null!;

    [Column("p_Email")]
    [StringLength(255)]
    public string? PEmail { get; set; }

    [Column("p_Address")]
    [StringLength(50)]
    public string? PAddress { get; set; }
}
