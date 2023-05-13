using CRM;
using CRM.Models;

namespace TestingProject
{
    [TestClass]
    public class UnitTest1
    {
        public static CrmRazorContext db = new CrmRazorContext();
        [TestMethod]
        public void TestingSearchOnTickets_SearchForWordTEST1()
        {
            string searchWord = "test1";
            List<Ticket> neededResult = new List<Ticket>() 
            { 
                new Ticket{TicketTitle = "test1"}
            };
            List<Ticket> realResult = Search.SearchOnTickets(searchWord);
            Assert.AreEqual(realResult.Count, neededResult.Count);
        }
    }
}