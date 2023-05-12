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
        Ticket? ticket { get; set; }
        static string EmailAddress = "uraxara.sox@yandex.ru";
        static string EmailPassword = "hqzdhdeeakjlqzzn";

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
        {
            Comment comment = new Comment();
            comment.CommentText = message;
            comment.CommentAuthorNavigation = Manager.currentUser;
            comment.Ticket = ticket;
            comment.DateAdded = DateTime.Now;
            ticket.Comments.Add(comment);
            db.SaveChanges();
            return new OkResult();
        }

        public IActionResult OnGetGetHistory()
        {
            return new OkResult();
        }

        public IActionResult OnPostPutData(string state, int[] users)
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
                string state = db.States.FirstOrDefault(x => x.StateId == ticket.State).Name;
                json.Add("state", state);
                var users_data = db.UsersForTickets
                    .Where(uft => uft.TicketId == id)
                    .Select(uft => uft.User)
                    .ToList();
                var users = JsonSerializer.Serialize(users_data);
                json.Add("users", users);
                var data = db.Comments.Where(x => x.TicketId == id).ToList();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var comments = JsonSerializer.Serialize(data, options);
                json.Add("comments", comments);
            }
            return new JsonResult(json);
        }
    }
}
