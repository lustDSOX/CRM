using CRM.Classes;
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
            string searchWord = "test 1";
            List<Ticket> neededResult = new List<Ticket>()
            {
                new Ticket{},
            };
            List<Ticket> realResult = Search.SearchOnTickets(searchWord);
            Assert.AreEqual(neededResult.Count, realResult.Count);
        }
        [TestMethod]
        public void TestingSearchOnRequseter_SearchForWordZimenkov_a()
        {
            string searchWord = "zimenkov_a";
            List<Requester> neededResult = new List<Requester>()
            {
                new Requester{}
            };
            List<Requester> realResult = Search.SearchOnRequester(searchWord);
            Assert.AreEqual(neededResult.Count, realResult.Count);
        }
    }
}