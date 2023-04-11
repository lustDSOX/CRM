using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class UsersForTicket
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int UserId { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
