using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class Receiver
{
    public int ReceiverId { get; set; }

    public string Name { get; set; } = null!;

    public int State { get; set; }

    public string Server { get; set; } = null!;

    public string IncommingMessageFolder { get; set; } = null!;

    public int? Port { get; set; }

    public string MailUsername { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public virtual State StateNavigation { get; set; } = null!;
}
