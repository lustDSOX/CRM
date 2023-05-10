using CRM.Models;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CRM.Pages
{
    public class AllRequestsModel : PageModel
    {
        private static CrmRazorContext db = Manager.db;
        public List<Ticket>? tickets { get; set; }
        public List<State>? states {get;set; }
        public List<User>? users {get;set; }
        Ticket? ticket { get; set; }
        static string EmailAddress = "uraxara.sox@yandex.ru";
        static string EmailPassword = "hqzdhdeeakjlqzzn";

        public void OnGet(int id)
        {
            states = db.States.ToList();
            users = db.Users.ToList();
            if (id == -1)
            {
                tickets = db.Tickets.ToList();
            }
            else
            {
                tickets = db.UsersForTickets
                .Where(uft => uft.UserId == id)
                .Select(uft => uft.Ticket)
                .ToList();
            }
        }



        void GetMailArray()
        {
            //using (var client = new ImapClient())
            //{
            //    client.Connect("imap.yandex.ru", 993, true);
            //    client.Authenticate(EmailAddress, EmailPassword);
            //    var inbox = client.GetFolder("INBOX");
            //    inbox.Open(FolderAccess.ReadOnly);
            //    var messages = inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId).Reverse();
            //    allMailSubject = messages.Select(m => m.Envelope.Subject).ToArray();
            //}
        }

        public IActionResult OnGetGetMailData(int i)
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.yandex.ru", 993, true);
                client.Authenticate(EmailAddress, EmailPassword);
                var inbox = client.GetFolder("INBOX");
                inbox.Open(FolderAccess.ReadOnly);
                var mes = inbox.GetMessage(inbox.Count - i - 1);
                string mail = mes.HtmlBody;
                string subj = mes.Subject;
                string json = JsonConvert.SerializeObject(new { mail, subj });
                return Content(json, "application/json");
            }
        }

        public IActionResult OnPostPutComment(string message)
        {
            Comment comment = new Comment();
            comment.CommentText = message;
            comment.CommentAuthorNavigation = Manager.currentUser;
            comment.Ticket = ticket;
            comment.DateAdded= DateTime.Now;
            ticket.Comments.Add(comment);
            db.SaveChanges();
            return new OkResult();
        }

        public IActionResult OnGetGetHistory()
        {
            return new OkResult();
        }

        public IActionResult OnPostPutData(string state,int[] users)
        {
            ticket.StateNavigation = db.States.FirstOrDefault(x => x.Name == state);
            var currentUsers = db.UsersForTickets.Where(x => x.TicketId == ticket.TicketId).Select(x => x.UserId).ToList();
            var usersToRemove = currentUsers.Except(users);
            foreach (var userId in usersToRemove)
            {
                var userForTicket = db.UsersForTickets
                    .SingleOrDefault(uft => uft.TicketId == ticket.TicketId && uft.UserId == userId);
                if (userForTicket != null)
                {
                    db.UsersForTickets.Remove(userForTicket);
                }
            }
            var usersToAdd = users.Except(currentUsers);
            foreach (var userId in usersToAdd)
            {
                var userForTicket = new UsersForTicket();
                userForTicket.UserId = userId;
                userForTicket.TicketId = ticket.TicketId;
                db.UsersForTickets.Add(userForTicket);
            }
            db.SaveChanges();

            return new OkResult();
        }

        public IActionResult OnGetData()
        {
            JsonObject json = new JsonObject();
            byte[] compressedBytes = ticket.TicketDesciption;
            using (var input = new MemoryStream(compressedBytes))
            {
                using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(gzip))
                    {
                        string html = reader.ReadToEnd();
                        json.Add("desciption", html);
                    }
                }
            }
            string requester = ticket.RequesterNavigation.Email;
            string state = ticket.StateNavigation.Name;
            string users = "";
            foreach(UsersForTicket usersForTicket in ticket.UsersForTickets)
            {
                users += db.Users.Find(usersForTicket.UserId).Name + ";";
            }
            var data = ticket.Comments.ToList();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var comments = System.Text.Json.JsonSerializer.Serialize(data, options);
            json.Add("comments", comments);
            return new JsonResult(json);
        }
    }
}
