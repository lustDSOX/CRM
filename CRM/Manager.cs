using CRM.Models;

namespace CRM
{
    public class Manager
    {
        public static CrmRazorContext db  = new CrmRazorContext();
        public static User currentUser { get; set; }

    }
}
