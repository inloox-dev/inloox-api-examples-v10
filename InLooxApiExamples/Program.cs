using Default;
using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using InLooxnowClient.Examples;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InLooxnowClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endPoint = new Uri("https://app.inlooxnow.com/");
            var endPointOdata = new Uri(endPoint, "odata/");

            var username = "user@inloox.com";
            var password = "";

            var tokenResponse = ODataBasics.GetToken(endPoint, username, password)
                .Result;

            // Example for logon with multiple accounts
            //tokenResponse = Logon.LogonMultipleAccounts(endPoint, username,
            //     password, "accountName");

            if (tokenResponse?.AccessToken == null)
            {
                Console.WriteLine("Login invalid");
                Console.ReadLine();
                return;
            }

            // get Current Contact info
            var context = ODataBasics.GetInLooxContext(endPointOdata,
                tokenResponse.AccessToken);

            var contact = await GetCurrentContact(context);
            Console.WriteLine($"Username: {contact.Name}");

            Console.WriteLine("----Example 1: Update custom field on document ----");
            var documentOperations = new DocumentQueries(context);
            documentOperations.UpdateDocumentCustomField().Wait();

            Console.WriteLine("----Example 2: Query, update & create InLoox task ----");
            var taskOperations = new TaskQueries(context);
            await taskOperations.QueryInLooxTask();
            var task = await taskOperations.CreateInLooxTask();
            await taskOperations.UpdateInLooxTask(task);

            Console.WriteLine("----Example 3: Create user and authentication ---");
            var ct = new ContactQueries(context);
            await ct.CreateUserAndAuth("user1", "user12@inloox.com", "SecurePassword_1");

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static async Task<Contact> GetCurrentContact(Container ctx)
        {
            var userRequest = ctx.contact.getauthenticated();
            var users = await userRequest.ExecuteAsync();
            return users.First();
        }
    }
}
