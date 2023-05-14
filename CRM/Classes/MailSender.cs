using CRM.Models;
using MailKit.Net.Smtp;
using MimeKit;

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
        public static async void SendUserSetOnTicket(Ticket ticket)
        {
            string message = $"<div>Заголовок: {ticket.TicketTitle}</div>\n" +
                             $"<div>Инициатор запроса: {ticket.RequesterNavigation.Email}</div>\n" +
                             $"<div>Дата открытия: {ticket.OpenDate}</div>\n" +
                             $"<div>Статус: Новая</div>\n" +
                             $"<div>Назначенные сотрудники: </div>\n" +
                             $"<div>Описание:\n {EncryptDecrypt.Decrypt(ticket.TicketDesciption)}</div>" +
                             $"<div>Комментарии:</div>\n";
            string usersOnTicket = "";
            foreach (var user in ticket.UsersForTickets.ToList())
            {
                usersOnTicket += $"{user.User.Name} ";
            }
            message.Insert(message.LastIndexOf("Назначенные сотрудники: "), usersOnTicket);
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
