using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string TicketTitle { get; set; } = null!;

    public string TicketDesciption { get; set; } = null!;

    public DateTime OpenDate { get; set; }

    public DateTime LastChanged { get; set; }

    public int State { get; set; }

    public int Requester { get; set; }

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User RequesterNavigation { get; set; } = null!;

    public virtual State StateNavigation { get; set; } = null!;

    public virtual ICollection<UsersForTicket> UsersForTickets { get; set; } = new List<UsersForTicket>();
}
