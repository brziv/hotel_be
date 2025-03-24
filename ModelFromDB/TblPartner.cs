using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblPartner
{
    public Guid PPartnerId { get; set; }

    public string PPartnerName { get; set; } = null!;

    public string? PPartnerType { get; set; }

    public string PPhoneNumber { get; set; } = null!;

    public string? PEmail { get; set; }

    public string? PAddress { get; set; }
}
