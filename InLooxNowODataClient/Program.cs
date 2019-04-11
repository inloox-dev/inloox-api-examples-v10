using Default;
using InLooxnowClient.Examples;
using InLooxOData;
using IQmedialab.InLoox.Data.BusinessObjects;
using System;
using System.Linq;

namespace InLooxnowClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var endPoint = new Uri("https://app.inlooxnow.com/");
            var endPointOdata = new Uri(endPoint, "odata/");

            var username = "user@inloox.com";
            var password = "";

            var tokenResponse = ODataBasics.GetToken(endPoint, username, password)
                .Result;

            // Example for logon with multiple accounts
            //tokenResponse = Logon.LogonMultipleAccounts(endPoint, username, password, "accountName");

            if (tokenResponse?.AccessToken == null)
            {
                Console.WriteLine("Login invalid");
                Console.ReadLine();
                return;
            }

            // get Current Contact info
            var context = ODataBasics.GetInLooxContext(endPointOdata,
                tokenResponse.AccessToken);

            var contact = GetCurrentContact(context);
            Console.WriteLine($"Username: {contact.Name}");

            Console.WriteLine("----Example 1: Update Custom field on document  ----");
            var documentOperations = new DocumentQueries(context);
            documentOperations.UpdateDocumentCustomField();

            Console.WriteLine("----Example 2: Query,Update & create InLoox task----");
            var taskOperations = new TaskQueries(context);
            taskOperations.QueryInLooxTask();
            var task = taskOperations.CreateInLooxTask();
            taskOperations.UpdateInLooxTask(task);

            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static Contact GetCurrentContact(Container ctx)
        {
            var user = ctx.contact.getauthenticated();
            return user.First();
        }
    }
}
