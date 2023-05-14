using CRM.Models;
using System.IO.Compression;
using System.Text;

namespace CRM.Classes
{
    public class Search
    {
        public static CrmRazorContext db = new CrmRazorContext();
        public static List<Ticket> SearchOnTickets(string searchWord)
        {
            List<Ticket> resultList = new List<Ticket>();
            searchWord = searchWord.ToLower();
            foreach (var ticket in db.Tickets.ToList())
            {
                if (ticket.TicketTitle.ToLower().Contains(searchWord) ||
                    EncryptDecrypt.Decrypt(ticket.TicketDesciption).ToLower().Contains(searchWord) ||
                    ticket.RequesterNavigation.Email.ToLower() == searchWord ||
                    ticket.RequesterNavigation.PhoneNumber.ToLower() == searchWord)
                {
                    resultList.Add(ticket);
                }
            }
            return resultList;
        }
        public static List<Requester> SearchOnRequester(string searchWord)
        {
            List<Requester> resultList = new List<Requester>();
            searchWord = searchWord.ToLower();
            foreach (var requester in db.Requesters.ToList())
            {
                if (requester.Email.ToLower().Contains(searchWord) ||
                    requester.PhoneNumber.ToLower().Contains(searchWord))
                {
                    resultList.Add(requester);
                }
            }
            return resultList;
        }
    }
}
