using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class State
{
    public bool StateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
