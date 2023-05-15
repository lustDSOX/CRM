using CRM.Models;
using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics;

namespace CRM.Classes
{
    public class MailSender
    {
        public static async void SendInitTicket(Ticket ticket)
        {
            string message = $"<div>Заголовок: {ticket.TicketTitle}</div>\n" +
                             $"<div>Инициатор запроса: {ticket.RequesterNavigation.Email}</div>\n" +
                             $"<div>Дата открытия: {ticket.OpenDate}</div>\n" +
                             $"<div>Статус: Новая</div>\n" +
                             $"<div>Описание:\n {EncryptDecrypt.Decrypt(ticket.TicketDesciption)}</div>" +
                             $"<div>Комментарии:</div>\n";
            foreach (var comment in ticket.Comments.Reverse())
            {
                message += $"<div>{comment.DateAdded} {comment.CommentAuthorNavigation}: {comment.CommentText}</div>\n";
            }

            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ManDOM", "zni@cit-nnov.ru"));
            emailMessage.To.Add(new MailboxAddress("", ticket.RequesterNavigation.Email));
            emailMessage.Subject = $"Заявка #{ticket.TicketId} создана";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            await ConnectAndSendMessage(emailMessage);
        }
        public static async void SendUserSetOnTicketORAddedComment(Ticket ticket, bool _isCalledFromSave)
        {
            bool isCalledFromSave = _isCalledFromSave;
            string message = $"<div>Заголовок: {ticket.TicketTitle}</div>\n" +
                                 $"<div>Инициатор запроса: {ticket.RequesterNavigation.Email}</div>\n" +
                                 $"<div>Дата открытия: {ticket.OpenDate}</div>\n" +
                                 $"<div>Статус: Новая</div>\n" +
                                 $"<div>Назначенные сотрудники: </div>\n" +
                                 $"<div>Описание:\n {EncryptDecrypt.Decrypt(ticket.TicketDesciption)}</div>" +
                                 $"<br><br>" +
                                 $"<div>Комментарии:</div>\n";
            string usersOnTicket = "";
            foreach (var user in ticket.UsersForTickets.Where(tk => tk.Ticket == ticket).ToList())
            {
                usersOnTicket += $"<div>{user.User.Name}</div>\n";
            }

            message = message.Insert(message.IndexOf("<div>Назначенные сотрудники:") + 28, usersOnTicket);

            foreach (var comment in ticket.Comments.Reverse())
            {
                message += $"<div>{comment.DateAdded} {comment.CommentAuthorNavigation}: {comment.CommentText}</div>\n";
            }

            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ManDOM", "zni@cit-nnov.ru"));
            emailMessage.To.Add(new MailboxAddress("", ticket.RequesterNavigation.Email));
            if (isCalledFromSave == true)
            {
                emailMessage.Subject = $"На заявку #{ticket.TicketId} назначены сотрудники";
            }
            else
            {
                emailMessage.Subject = $"К заявке #{ticket.TicketId} добавлен комментарий";
            }

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            await ConnectAndSendMessage(emailMessage);
        }
        public static async void SendCompleteTicket(Ticket ticket)
        {
            string message = $"<div>Заголовок: {ticket.TicketTitle}</div>\n" +
                             $"<div>Инициатор запроса: {ticket.RequesterNavigation.Email}</div>\n" +
                             $"<div>Дата открытия: {ticket.OpenDate}</div>\n" +
                             $"<div>Дата закрытия: {DateTime.Now}</div>\n" +
                             $"<div>Статус: Закрыта</div>\n" +
                             $"<div>Описание:\n {EncryptDecrypt.Decrypt(ticket.TicketDesciption)}</div>" +
                             $"<div>Комментарии:</div>\n";
            foreach (var comment in ticket.Comments.Reverse())
            {
                message += $"<div>{comment.DateAdded} {comment.CommentAuthorNavigation}: {comment.CommentText}</div>\n";
            }
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("ManDOM", "zni@cit-nnov.ru"));
            emailMessage.To.Add(new MailboxAddress("", ticket.RequesterNavigation.Email));
            emailMessage.Subject = $"Заявка #{ticket.TicketId} закрыта";

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            await ConnectAndSendMessage(emailMessage);
        }

        private static async Task ConnectAndSendMessage(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("mail.cit-nnov.ru", 587, false);
                await client.AuthenticateAsync("zni@cit-nnov.ru", "xozetux/");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
