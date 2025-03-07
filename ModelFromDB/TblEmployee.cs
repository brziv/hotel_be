using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Employees")]
[Index("EEmail", Name = "UQ__tbl_Empl__3908DC82E918A11D", IsUnique = true)]
public partial class TblEmployee
{
    [Key]
    [Column("e_EmployeeID")]
    public Guid EEmployeeId { get; set; }

    [Column("e_FirstName")]
    [StringLength(50)]
    public string EFirstName { get; set; } = null!;

    [Column("e_LastName")]
    [StringLength(50)]
    public string ELastName { get; set; } = null!;

    [Column("e_Email")]
    [StringLength(100)]
    public string EEmail { get; set; } = null!;

    [Column("e_PhoneNumber")]
    [StringLength(15)]
    public string? EPhoneNumber { get; set; }

    [Column("e_Position")]
    [StringLength(50)]
    public string EPosition { get; set; } = null!;

    [Column("e_Salary", TypeName = "decimal(10, 2)")]
    public decimal ESalary { get; set; }
}
