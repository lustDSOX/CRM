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
using CRM.Models.Db_classes;
using CRM.Models;

namespace CRM.Pages
{
	public class MainPageModel : PageModel
	{
		static User current_user = null;
        private static CrmRazorContext db = new CrmRazorContext();
        private static List<User> Users = db.Users.ToList();
		public string username;
        public void OnGet(string user_id)
		{
			foreach(var user in Users)
			{
				if(user.UserId == int.Parse(user_id))
				{
					current_user = user;
                    username = user.Name;
				}
			}
		}
	}
}
