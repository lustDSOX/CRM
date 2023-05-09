using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Requester
{
    public int ReqId { get; set; }

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
