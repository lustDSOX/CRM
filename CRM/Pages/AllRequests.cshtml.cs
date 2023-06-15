using CRM.Classes;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public List<State>? states { get; set; }
        public List<User>? users { get; set; }
        static Ticket? ticket { get; set; } // выбранная заявка

        static int cur_user;

        public IActionResult OnPostPutData(int stage, string respons)//Получает id стадии и назначенных чевовекав
        {
            string new_log = DateTime.Now.ToString() + " --- ";
            ticket.LastChanged = DateTime.Now;
            if (respons != null)
            {
                new_log += "Изменение ответственных с ( ";

                int[] users_id = respons.Split(',').Select(int.Parse).ToArray();

                // Удаляем лишних работников из заявки
                List<UsersForTicket> us_del = db.UsersForTickets.Where(u => u.TicketId == ticket.TicketId).ToList();
                foreach (var u in us_del)
                {
                    new_log += u.User.Name + "[" + u.UserId + "]    ";
                }
                new_log += " ) на ( ";
                db.UsersForTickets.RemoveRange(us_del);
                List<UsersForTicket> us_add = new List<UsersForTicket>();
                foreach (var user in users_id)
                {
                    UsersForTicket uft = new UsersForTicket();
                    uft.TicketId = ticket.TicketId;
                    uft.UserId = user;
                    uft.User = db.Users.Find(user);
                    us_add.Add(uft);
                    new_log += uft.User.Name + "[" + uft.UserId + "]    ";
                }
                new_log += " )";
                if (us_del.Equals(us_add))
                    new_log = "";
                db.UsersForTickets.AddRange(us_add);

                MailSender.SendUserSetOnTicketORAddedComment(ticket, true);
            }
            else
            {
                new_log += "Удалены все ответственные";
                var us_del = db.UsersForTickets.Where(u => u.TicketId == ticket.TicketId).ToList();
                db.UsersForTickets.RemoveRange(us_del);
            }
            //Сохранение изменений статуса
            if (ticket.State != stage)
            {
                new_log += "      Изменение статуса с \"" + ticket.StateNavigation.Name + "\" на \"" + db.States.Find(stage).Name + "\"";
            }
            ticket.State = stage;
            if (ticket.State == 5)
            {
                MailSender.SendCompleteTicket(ticket);
            }
            db.SaveChanges();

            string existingContent;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs/", ticket.TicketId.ToString() + ".txt");

            if (!System.IO.File.Exists(filePath))
            {
                // Создаем файл, если его не существует
                using (StreamWriter fileWriter = System.IO.File.CreateText(filePath))
                {
                    // Можно записать начальное содержимое файла, если требуется
                }
            }
            using (StreamReader sr = new StreamReader(filePath))
            {
                existingContent = sr.ReadToEnd();
            }
            new_log += "\n";
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(new_log);
                sw.Write(existingContent);
            }
            return new OkResult();
        }
        public void OnGet(int id)
        {
            states = db.States.ToList();
            users = db.Users.ToList();
            cur_user = id;

        }

        public IActionResult OnPostPutComment(string message)
        {
            if (message != null)
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
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logs/", ticket.TicketId.ToString() + ".txt");
            string existingContent;
            using (StreamReader sr = new StreamReader(filePath))
            {
                existingContent = sr.ReadToEnd();
            }
            return Content(existingContent);
        }

        public IActionResult OnGetSetData(int id)
        {
            JsonObject json = new JsonObject();
            using (CrmRazorContext dbContext = new CrmRazorContext())
            {
                ticket = db.Tickets.Find(id);
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
                string requester = dbContext.Requesters.FirstOrDefault(x => x.ReqId == ticket.Requester).Email;
                json.Add("requester", requester);
                int state = dbContext.States.FirstOrDefault(x => x.StateId == ticket.State).StateId;
                json.Add("state", state);
                var users_data = dbContext.UsersForTickets
                    .Where(uft => uft.TicketId == id)
                    .Select(uft => uft.User.UserId)
                    .ToList();
                var users = JsonSerializer.Serialize(users_data);
                json.Add("users", users);
                var data_comm = dbContext.Comments.Where(x => x.TicketId == id).ToList();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var comments = JsonSerializer.Serialize(data_comm, options);
                json.Add("comments", comments);
                var data_attach = dbContext.Attachments
                    .Where(x => x.TicketId == id)
                    .Select(x => x.AttachmentPath)
                    .ToList();
                var attach = JsonSerializer.Serialize(data_attach, options);
                json.Add("attach", attach);
            }
            return new JsonResult(json);
        }

        public IActionResult OnGetGetData()
        {
            if (cur_user == -1)
            {
                var data = db.Tickets.Select(x => new { x.TicketId, x.StateNavigation, x.TicketTitle }).ToList();
                data.Reverse();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var json = JsonSerializer.Serialize(data, options);
                return new JsonResult(json);
            }
            else
            {
                var data = db.UsersForTickets.Where(x => x.UserId == cur_user).Select(x => new { x.Ticket.TicketId, x.Ticket.StateNavigation, x.Ticket.TicketTitle }).ToList();
                data.Reverse();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };
                var json = JsonSerializer.Serialize(data, options);
                return new JsonResult(json);
            }
        }
    }
}
