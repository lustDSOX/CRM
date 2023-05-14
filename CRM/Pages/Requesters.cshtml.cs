using CRM.Classes;
using CRM.Models;
using MailKit.Net.Imap;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace CRM.Pages
{
    public class RequestersModel : PageModel
    {
        CrmRazorContext db = Manager.db;
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

        public IActionResult OnGetSetData(int id)
        {
            Requester requester= db.Requesters.FirstOrDefault(x => x.ReqId == id);
            var json = JsonConvert.SerializeObject(requester);
            return new JsonResult(json);
        }
    }
}
