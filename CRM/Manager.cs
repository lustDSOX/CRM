using CRM.Models;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MailKit.Security;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using System.IO.Compression;

namespace CRM
{
    public class Manager
    {
        public static CrmRazorContext db = new CrmRazorContext();
        public static List<Requester> requesters = db.Requesters.ToList();
        public static User currentUser { get; set; }
        static bool isActive = true;
        public static void GetTickets1Min()
        {
            // устанавливаем метод обратного вызова
            TimerCallback tm = new TimerCallback(GetTickets);
            // создаем таймер
            if (isActive)
            {
                Timer timer = new Timer(tm, isActive, 0, 60000);
                //GetTickets();
            }
            isActive = false;
        }
        public static void GetTickets(object obj)
        {
            var receivers = db.Receivers.ToList();
            foreach (var receiver in receivers)
            {
                if (receiver.Active == true)
                {
                    GetEmailsFromReceiver(receiver);
                }
            }
        }
        public static void GetTickets()
        {
            var receivers = db.Receivers.ToList();
            foreach (var receiver in receivers)
            {
                if (receiver.Active == true)
                {
                    GetEmailsFromReceiver(receiver);
                }
            }
        }
        public static void GetEmailsFromReceiver(Receiver receiver)
        {
            using (var client = new ImapClient())
            {
                //string connection = "imap." + receiver.MailUsername.Substring(receiver.MailUsername.IndexOf("@") + 1);
                client.Connect("mail.cit-nnov.ru", 143, false);
                client.Authenticate(receiver.MailUsername, receiver.UserPassword);
                var inbox = client.GetFolder(receiver.IncommingMessageFolder);
                inbox.Open(FolderAccess.ReadWrite);
                var uids = inbox.Search(SearchQuery.SentSince(DateTime.Now.AddDays(-1)).And(SearchQuery.NotDeleted));
                foreach (var item in inbox.Fetch(uids, MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure))
                {
                    Ticket ticket = new Ticket();
                    ticket.TicketTitle = item.NormalizedSubject; //Заголовок письма

                    byte[] tempHtml = Encoding.UTF8.GetBytes(inbox.GetMessage(item.UniqueId).HtmlBody.ToCharArray());

                    using (var output = new MemoryStream())
                    {
                        using (var gzip = new GZipStream(output, CompressionMode.Compress))
                        {
                            gzip.Write(tempHtml, 0, tempHtml.Length);
                        }
                        byte[] compressedBytes = output.ToArray();
                        ticket.TicketDesciption = compressedBytes; //Сжатое описание письма
                    }

                    ticket.OpenDate = item.Date.UtcDateTime; //Дата открытия заявки

                    ticket.LastChanged = item.Date.UtcDateTime; //Дата последнего изменения, при создании заявки совпадает

                    ticket.State = 1; //Статус. 1 - Новая

                    string temp = item.Envelope.From.ToString(); //Обработка почты инициатора запроса
                    temp = temp.Substring(temp.IndexOf('<') + 1);
                    temp = temp.Trim('>');
                    Requester tempReq = new Requester { Email = temp };
                    if (requesters.Any(req => req.Email == tempReq.Email))
                    {
                        ticket.RequesterNavigation = requesters.Where(tmp1 => tmp1.Email == temp).Single();
                    }
                    else
                    {
                        ticket.RequesterNavigation = tempReq; //Создание и запись инициатора запроса (от кого письмо)
                    }

                    inbox.AddFlags(item.UniqueId, MessageFlags.Deleted, true); //Удаление письма
                    db.Tickets.Add(ticket);
                }
                inbox.Close();
                client.Disconnect(true);
                db.SaveChanges();
            }
        }
    }
}
