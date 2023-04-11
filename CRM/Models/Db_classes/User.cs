using System;
using System.Collections.Generic;

namespace CRM.Models.Db_classes;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int Role { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<UsersForTicket> UsersForTickets { get; set; } = new List<UsersForTicket>();
}
