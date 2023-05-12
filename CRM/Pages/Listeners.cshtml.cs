using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CRM.Pages
{
    public class ListenersModel : PageModel
    {
        private static CrmRazorContext db = Manager.db;
        static Receiver receiver = new Receiver();
        static bool is_new = false;
        public void OnGet()
        {
        }

        public IActionResult OnGetGetData()
        {
            List<Receiver> data;
            lock (db)
            {
                data = db.Receivers.ToList();
            }
            return new JsonResult(data);
        }
        public IActionResult OnGetPutData(string name, bool state,string server, string address, string folder,string comment,string password)
        {
            receiver.Name = name;
            receiver.Active = state;
            receiver.MailUsername = address;
            receiver.MailUsername = server;
            receiver.IncommingMessageFolder = folder;
            receiver.Comment = comment;
            receiver.UserPassword = password;
            //if(is_new)
            //    db.Receivers.Add(receiver);
            //db.SaveChanges();
            return new OkResult();
        }

        public IActionResult OnGetDeleteData() { 
            db.Receivers.Remove(receiver);
            db.SaveChanges();
            return new OkResult();
        }

        public IActionResult OnGetSetData(int id)
        {
            if (id == -1)
            {
                receiver = new Receiver();
                is_new = true;
                return StatusCode(StatusCodes.Status204NoContent);
            }
            is_new = false;
            receiver = db.Receivers.FirstOrDefault(x => x.ReceiverId == id);
            var json = JsonConvert.SerializeObject(receiver);
            return new JsonResult(json);
        }
    }
}
