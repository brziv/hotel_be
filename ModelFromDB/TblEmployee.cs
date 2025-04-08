using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblEmployee
{
    public Guid EEmployeeId { get; set; }

    public string EFirstName { get; set; } = null!;

    public string ELastName { get; set; } = null!;

    public string EEmail { get; set; } = null!;

    public string? EPhoneNumber { get; set; }

    public string EPosition { get; set; } = null!;

    public decimal ESalary { get; set; }

    public string? EUserId { get; set; }
    public IdentityUser? User { get; set; }
}
