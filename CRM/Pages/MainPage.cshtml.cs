using MailKit;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Net.Mail;
using System.Text.Json;

namespace CRM.Pages
{
    public class MainPageModel : PageModel
    {
        public string[] allMailSubject { get; private set; } = new string[0];
        static string EmailAddress = "uraxara.sox@yandex.ru";
        static string EmailPassword = "hqzdhdeeakjlqzzn";

        [Authorize]
        public void OnGet()
        {
            GetMailArray();
        }
        void GetMailArray()
        {
            using (var client = new ImapClient())
            {
                client.Connect("imap.yandex.ru", 993, true);
                client.Authenticate(EmailAddress, EmailPassword);
                var inbox = client.GetFolder("INBOX");
                inbox.Open(FolderAccess.ReadOnly);
                var messages = inbox.Fetch(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId).Reverse();
                allMailSubject = messages.Select(m => m.Envelope.Subject).ToArray();
            }
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
    }
}
