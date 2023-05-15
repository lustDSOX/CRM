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
        Requester? requester;
        public void OnGet()
        {
        }
        public IActionResult OnGetGetData()
        {
            List<Requester> data;
            lock (db)
            {
                data = db.Requesters.ToList();
            }
            return new JsonResult(data);
        }

        public IActionResult OnGetSetData(int id)
        {
            requester= db.Requesters.FirstOrDefault(x => x.ReqId == id);
            var json = JsonConvert.SerializeObject(requester);
            return new JsonResult(json);
        }

        public IActionResult OnPostPutData(string number)
        {
            requester.PhoneNumber = number;
            db.SaveChanges();
            return new OkResult();
        }
    }
}
