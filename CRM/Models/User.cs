using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Role { get; set; }

    public string? AvatarUrl { get; set; }

    public string Login { get; set; } = null!;

    public bool? Working { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Role RoleNavigation { get; set; } = null!;

    public virtual ICollection<UsersForTicket> UsersForTickets { get; set; } = new List<UsersForTicket>();
}
