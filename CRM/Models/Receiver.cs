using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Receiver
{
    public int ReceiverId { get; set; }

    public string? Name { get; set; }

    public bool Active { get; set; }

    public string IncommingMessageFolder { get; set; } = null!;

    public string MailUsername { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public string? Comment { get; set; }
}
