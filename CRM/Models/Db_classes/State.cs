using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class State
{
    public int StateId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Receiver> Receivers { get; set; } = new List<Receiver>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
