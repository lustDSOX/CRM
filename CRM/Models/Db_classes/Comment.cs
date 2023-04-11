using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class Comment
{
    public int CommentId { get; set; }

    public int TicketId { get; set; }

    public DateTime DateAdded { get; set; }

    public int CommentAuthor { get; set; }

    public string CommentText { get; set; } = null!;

    public virtual User CommentAuthorNavigation { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
