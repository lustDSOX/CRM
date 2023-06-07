using CRM.Classes;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CRM.Pages
{
    public class RequestersModel : PageModel
    {
        CrmRazorContext db = Manager.db;
        Requester? requester;
        public void OnGet()
        {
        }
        public IActionResult OnGetGetData()
        {
            List<Requester> data;
            data = db.Requesters.AsNoTracking().ToList();
            return new JsonResult(data);
        }

        public IActionResult OnGetSetData(int id)
        {
            requester = db.Requesters.FirstOrDefault(x => x.ReqId == id);
            var req = new { requester.PhoneNumber, requester.Email };
            var tickets = db.Tickets.Where(x => x.Requester == id).Select(x => new { x.TicketId, x.StateNavigation,  x.TicketTitle, x.OpenDate }).ToList();
            List<object> uft = new List<object>();
            foreach (var item in tickets)
            {
                var users = db.UsersForTickets.Where(x => x.TicketId == item.TicketId).Select(x=>x.User.Name).ToList();
                uft.Add(users);
            }
            if (tickets != null)
            {
                var combinedObject = new
                {
                    Requester = req,
                    Tickets = tickets,
                    UFT = uft
                };
                string json = JsonConvert.SerializeObject(combinedObject);
                return new JsonResult(json);
            }
            else
            {
                var combinedObject = new
                {
                    Requester = req,
                    Tickets = new List<object>()
                };
                string json = JsonConvert.SerializeObject(combinedObject);
                return new JsonResult(json);
            }
        }

        public IActionResult OnPostPutData(string number)
        {
            requester.PhoneNumber = number;
            db.SaveChanges();
            return new OkResult();
        }
    }
}
