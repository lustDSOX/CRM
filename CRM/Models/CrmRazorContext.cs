using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRM.Models;

public partial class CrmRazorContext : DbContext
{
    public CrmRazorContext()
    {
    }

    public CrmRazorContext(DbContextOptions<CrmRazorContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Receiver> Receivers { get; set; }

    public virtual DbSet<Requester> Requesters { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersForTicket> UsersForTickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ngknn.ru;Database=CRM_razor;User Id=33П;Password=12357;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");
            entity.Property(e => e.AttachmentPath)
                .IsUnicode(false)
                .HasColumnName("attachment_path");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachments_Tickets");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommentAuthor).HasColumnName("comment_author");
            entity.Property(e => e.CommentText)
                .IsUnicode(false)
                .HasColumnName("comment_text");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("date_added");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.CommentAuthorNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.CommentAuthor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Comments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Tickets");
        });

        modelBuilder.Entity<Receiver>(entity =>
        {
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Comment)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.IncommingMessageFolder)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("incomming_message_folder");
            entity.Property(e => e.MailUsername)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mail_username");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_password");
        });

        modelBuilder.Entity<Requester>(entity =>
        {
            entity.HasKey(e => e.ReqId);

            entity.Property(e => e.ReqId).HasColumnName("req_id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.Property(e => e.StateId).HasColumnName("state_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.LastChanged)
                .HasColumnType("datetime")
                .HasColumnName("last_changed");
            entity.Property(e => e.OpenDate)
                .HasColumnType("datetime")
                .HasColumnName("open_date");
            entity.Property(e => e.Requester).HasColumnName("requester");
            entity.Property(e => e.State).HasColumnName("state");
            entity.Property(e => e.TicketDesciption)
                .IsUnicode(false)
                .HasColumnName("ticket_desciption");
            entity.Property(e => e.TicketTitle)
                .IsUnicode(false)
                .HasColumnName("ticket_title");

            entity.HasOne(d => d.RequesterNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Requester)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_Requesters");

            entity.HasOne(d => d.Requester1).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Requester)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_Users");

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.State)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tickets_States");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AvatarUrl)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Login)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UsersForTicket>(entity =>
        {
            entity.ToTable("Users_for_tickets");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.UsersForTickets)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_for_tickets_Tickets");

            entity.HasOne(d => d.User).WithMany(p => p.UsersForTickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_for_tickets_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
