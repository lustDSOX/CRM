using CRM.Models;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit;
using MailKit.Security;
using System.Text;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using System.IO.Compression;
using MimeKit;

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

                try
                {
                    client.Connect(receiver.MailServer, 143, false);
                }
                catch (Exception)
                {
                    client.Connect(receiver.MailServer, 993, true);
                }
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
                    db.Tickets.Add(ticket);
                    db.SaveChanges(); //Создали заявку, чтобы можно было на неё ссылаться из Attachments
                    if (item.Attachments != null && item.Attachments.Count() > 0)
                    {
                        foreach (var attachment in item.Attachments.OfType<BodyPartBasic>())
                        {
                            var part = (MimePart)inbox.GetBodyPart(item.UniqueId, attachment); //Получаем тело с вложениями
                            var pathDir = Path.Combine(Environment.CurrentDirectory, "wwwroot\\attachments", db.Tickets.ToList().Last().TicketId.ToString());
                            if (!Directory.Exists(pathDir))
                            {
                                Directory.CreateDirectory(pathDir); //Создаём директорию под вложения
                            }

                            var path = Path.Combine(pathDir, part.FileName);
                            if (!File.Exists(path))
                            {
                                using (var stream = File.Create(path))
                                {
                                    part.Content.DecodeTo(stream); //Загружаем вложение в директорию
                                }
                            }
                            Attachment dbAttachment = new Attachment()
                            {
                                TicketId = db.Tickets.ToList().Last().TicketId, //Запихиваем все в таблицу заявок
                                AttachmentPath = path
                            };
                            db.Attachments.Add(dbAttachment);
                        }
                    }
                    inbox.AddFlags(item.UniqueId, MessageFlags.Deleted, true); //Удаление письма
                }
                inbox.Close();
                client.Disconnect(true);
                db.SaveChanges();
            }
        }
    }
}
