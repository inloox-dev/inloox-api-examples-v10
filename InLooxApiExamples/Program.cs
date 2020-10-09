using System;
using System.Linq;
using System.Threading.Tasks;
using Default;
using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using InLooxApiExamples.Examples;

namespace InLooxApiExamples
{
    class Program
    {
        private static readonly Uri EndPoint = new Uri("https://app.inlooxnow.de/");
        private static readonly Uri EndPointOdata = new Uri(EndPoint, "odata/");

        static async Task Main(string[] args)
        {
            var envToken = Environment.GetEnvironmentVariable("InLooxApiToken");
            var token = envToken ?? Logon.LogonOauth(EndPoint);

            if (token == null)
            {
                Console.WriteLine("Login invalid");
                Console.ReadLine();
            }

            Console.WriteLine("token: " + token);

            await StartRepl(args, token);
        }

        private static async Task StartRepl(string[] args, string token)
        {
            while (true)
            {
                Console.WriteLine("Enter Number to run Example (1-6) or q to exit:");
                var selection = args.Length == 1 ? args[0] : Console.ReadLine();
                if (selection.Trim().ToLower() == "q")
                    return;

                args = Array.Empty<string>();

                var number = int.Parse(selection);
                await RunExample(number, token);
                Console.WriteLine("done");
            }
        }

        private static async Task RunExample(int number, string token)
        {
            var context = ODataBasics.GetInLooxContext(EndPointOdata,
                token);
            var contact = await GetCurrentContact(context);

            Console.WriteLine($"Username: {contact.Name}");

            switch (number)
            {
                case 1:
                    Console.WriteLine("----Example 1: Update custom field on document ----");
                    var documentOperations = new DocumentExample(context);
                    await documentOperations.UpdateDocumentCustomField();
                    break;
                case 2:
                    Console.WriteLine("----Example 2: Query, update & create InLoox task ----");
                    var taskExample = new TaskExample(context);
                    await taskExample.QueryLastChangedInLooxTasks();

                    var task = await taskExample.CreateInLooxTask();
                    await taskExample.UpdateTaskName(task);
                    break;
                case 3:
                    Console.WriteLine("----Example 3: Create user and authentication ---");
                    var ct = new ContactExample(context);
                    await ct.CreateUserAndAuth("user1", "user12@inloox.com", "SecurePassword_1");
                    break;
                case 4:
                    Console.WriteLine("----Example 4: Patch custom expand on task---");
                    var ceq = new CustomExpandExample(context);
                    await ceq.PatchTaskCustomExpand();
                    break;
                case 5:
                    Console.WriteLine("----Example 5: Upload Document");
                    var documentExample = new DocumentExample(context);
                    await documentExample.AddExampleFileToProject();
                    break;
                case 6:
                    Console.WriteLine("----Example 6: CreateFolder");
                    documentExample = new DocumentExample(context);
                    await documentExample.CreateFolderAndListContent();
                    break;
                default:
                    Console.WriteLine($"Unkown Command {number}");
                    break;
            }
        }

        private static async Task<Contact> GetCurrentContact(Container ctx)
        {
            var userRequest = ctx.contact.getauthenticated();
            var users = await userRequest.ExecuteAsync();
            return users.First();
        }
    }
}
