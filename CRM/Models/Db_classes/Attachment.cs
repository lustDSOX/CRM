using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class Attachment
{
    public int AttachmentId { get; set; }

    public int TicketId { get; set; }

    public string AttachmentPath { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
