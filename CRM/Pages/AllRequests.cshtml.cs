using CRM.Classes;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CRM.Pages
{
    public class AllRequestsModel : PageModel
    {
        private static CrmRazorContext db = Manager.db;
        public List<Ticket>? tickets { get; set; }
        public List<State>? states { get; set; }
        public List<User>? users { get; set; }
        static Ticket? ticket { get; set; } // выбранна€ за€вка

        public IActionResult OnPostPutData(int stage, string respons)//ѕолучает id стадии и назначенных чевовекав
        {
            string[] responsArr = respons.Split(',');
            List<User> selectRespons = new List<User>(); //—писок назначенных сотрудников, которых необходимо сохранить
            if (selectRespons != null)
            {
                //—охранение выбранных отвественных
                foreach (var user in selectRespons)
                {
                    db.UsersForTickets.Add(new UsersForTicket { Ticket = ticket, User = user });
                }
                MailSender.SendUserSetOnTicket(ticket);
            }
            State selectedState = new State(); // ¬ыбранный статус за€вки

            //—охранение изменений статуса
            ticket.State = selectedState.StateId;
            if(ticket.State == 5)
            {
                MailSender.SendCompleteTicket(ticket);
            }
            db.SaveChanges();
            return new OkResult();
        }
        public void OnGet(int id)
        {
            states = db.States.ToList();
            users = db.Users.ToList();

            if (id == -1)
            {
                tickets = db.Tickets.AsNoTracking().ToList();
            }
            else
            {
                tickets = db.UsersForTickets
                    .Where(uft => uft.UserId == id)
                    .Select(uft => uft.Ticket)
                    .AsNoTracking()
                    .ToList();
            }

        }

        public IActionResult OnPostPutComment(string message)
        {   if (message != null)
            {
                Comment comment = new Comment();
                comment.CommentText = message;
                comment.CommentAuthorNavigation = Manager.currentUser;
                comment.Ticket = ticket;
                comment.DateAdded = DateTime.Now;
                ticket.Comments.Add(comment);
                db.SaveChanges();
            }
            return new OkResult();
        }

        public IActionResult OnGetGetHistory()
        {
            return new OkResult();
        }

        public IActionResult OnGetSetData(int id)
        {
            JsonObject json = new JsonObject();
            lock (db)
            {
                ticket = db.Tickets.FirstOrDefault(x => x.TicketId == id);
                json.Add("title", ticket.TicketTitle);
                json.Add("open_date", ticket.OpenDate);
                json.Add("last_changed", ticket.LastChanged);
                byte[] compressedBytes = ticket.TicketDesciption;
                try
                {
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
                }
                catch
                {
                    string result = Encoding.UTF8.GetString(compressedBytes);
                    json.Add("desciption", result);
                }
                string requester = db.Requesters.FirstOrDefault(x => x.ReqId == ticket.Requester).Email;
                json.Add("requester", requester);
                int state = db.States.FirstOrDefault(x => x.StateId == ticket.State).StateId;
                json.Add("state", state);
                var users_data = db.UsersForTickets
                    .Where(uft => uft.TicketId == id)
                    .Select(uft => uft.User.UserId)
                    .ToList();
                var users = JsonSerializer.Serialize(users_data);
                json.Add("users", users);
                var data_comm = db.Comments.Where(x => x.TicketId == id).ToList();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var comments = JsonSerializer.Serialize(data_comm, options);
                json.Add("comments", comments);
                var data_attach = db.Attachments
                    .Where(x=>x.TicketId==id)
                    .Select(x => x.AttachmentPath)
                    .ToList();
                var attach = JsonSerializer.Serialize(data_attach, options);
                json.Add("attach", attach);
            }
            return new JsonResult(json);
        }

        public IActionResult OnGetGetData()
        {
            var data = db.Tickets.Select(x=>new {x.TicketId,x.StateNavigation,x.TicketTitle}).ToList();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
            var json = JsonSerializer.Serialize(data, options);
            return new JsonResult(json);
        }
    }
}
