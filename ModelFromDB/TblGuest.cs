using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace hotel_be.ModelFromDB;

public partial class TblGuest
{
    public Guid GGuestId { get; set; }

    public string GFirstName { get; set; } = null!;

    public string GLastName { get; set; } = null!;

    public string? GEmail { get; set; }

    public string GPhoneNumber { get; set; } = null!;

    public string? GUserId { get; set; }
    public IdentityUser? User { get; set; }

    public virtual ICollection<TblBooking> TblBookings { get; set; } = new List<TblBooking>();
}
