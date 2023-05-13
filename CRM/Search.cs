using CRM.Models;
using System.IO.Compression;
using System.Text;

namespace CRM
{
    public class Search
    {
        public static List<Ticket> resultList = new List<Ticket>();
        public static CrmRazorContext db = new CrmRazorContext();
        public static List<Ticket> SearchOnTickets(string searchWord)
        {
            searchWord = searchWord.ToLower();
            string ticketDescription = "";
            foreach (var ticket in db.Tickets.ToList())
            {
                byte[] compressedBytes = ticket.TicketDesciption;
                try
                {
                    using (var input = new MemoryStream(compressedBytes))
                    {
                        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
                        {
                            using (var reader = new StreamReader(gzip))
                            {
                                ticketDescription = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    string result = Encoding.UTF8.GetString(compressedBytes);
                }
                try
                {
                    if (ticket.TicketTitle.ToLower().Contains(searchWord) || ticketDescription.ToLower().Contains(searchWord) || ticket.RequesterNavigation.Email.ToLower().Contains(searchWord))
                    {
                        resultList.Add(ticket);
                    }
                }
                catch (Exception)
                {
                }
            }
            return resultList;
        }
    }
}
