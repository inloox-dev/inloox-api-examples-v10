using System;
using System.Collections.Generic;
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
        private static readonly Uri EndPoint = new Uri("https://app.inlooxnow-beta.de/");
        private static readonly Uri EndPointOdata = new Uri(EndPoint, "odata/");

        private static List<Example> Examples { get; set; } = new List<Example>();

        static async Task Main(string[] args)
        {
            var envToken = Environment.GetEnvironmentVariable("InLooxAccessToken");
            var token = envToken ?? Logon.LogonOauth(EndPoint);

            if (token == null)
            {
                Console.WriteLine("Login invalid");
                Console.ReadLine();
            }

            Console.WriteLine("token: " + token);

            RegisterExamples();

            await StartRepl(args, token);
        }

        private static async Task StartRepl(string[] args, string token)
        {
            while (true)
            {
                PrintExamples();
                Console.WriteLine($"Enter Number to run Example (1 - {Examples.Count}) or q to exit:");
                var selection = args.Length == 1 ? args[0] : Console.ReadLine();
                if (selection.Trim().ToLower() == "q")
                    return;

                args = Array.Empty<string>();

                var number = int.Parse(selection);
                await RunExample(number, token);
                Console.WriteLine("done");
            }
        }

        private static void PrintExamples()
        {
            foreach (var x in Enumerable.Range(0, Examples.Count))
            {
                Console.WriteLine($"Example {x + 1}: {Examples[x].Description}");
            }
        }

        private static void RegisterExamples()
        {
            AddExample("Document: Update custom field", async (context) =>
            {
                var documentOperations = new DocumentExample(context);
                await documentOperations.UpdateDocumentCustomField();
            });

            AddExample("Tasks: Query, update & create", async (context) =>
            {
                var taskExample = new TaskExample(context);
                await taskExample.QueryLastChangedInLooxTasks();

                var task = await taskExample.CreateInLooxTask();
                await taskExample.UpdateTaskName(task);
            });

            AddExample("Authentication: Create user", async (context) =>
            {
                var ct = new ContactExample(context);
                await ct.CreateUserAndAuth("user1", "user12@inloox.com", "SecurePassword_1");
            });

            AddExample("Task: Update custom field", async (context) =>
            {
                var ceq = new CustomExpandExample(context);
                await ceq.PatchTaskCustomExpand();
            });

            AddExample("Document: Upload new file", async (context) =>
            {
                var documentExample = new DocumentExample(context);
                await documentExample.AddExampleFileToProject();
            });

            AddExample("Document: Create folder", async (context) =>
            {
                var documentExample = new DocumentExample(context);
                await documentExample.CreateFolderAndListContent();
            });

            AddExample("Document: Get latest changes", async (context) =>
            {
                var documentExample = new DocumentExample(context);
                await documentExample.GetLatestChanges();
            });

            AddExample("Document: Move Document", async (context) =>
            {
                var documentExample = new DocumentExample(context);
                await documentExample.MoveDocument();
            });

            AddExample("DocumentSync: Get latest cahnges", async (context) =>
            {
                var documentSyncExample = new DocumentSyncExample(context);
                await documentSyncExample.GetLatestChanges();
            });

            AddExample("Document: Rename document & folder", async (context) =>
            {
                var documentSyncExample = new DocumentExample(context);
                await documentSyncExample.RenameDocumentAndFolder();
            });

            AddExample("Document: Update file infos", async (context) =>
            {
                var documentSyncExample = new DocumentExample(context);
                await documentSyncExample.UpdateFileInfo();
            });
        }

        private static async Task RunExample(int number, string token)
        {
            var context = ODataBasics.GetInLooxContext(EndPointOdata,
                token);

            var contact = await GetCurrentContact(context);

            Console.WriteLine($"Username: {contact.Name}");

            if (number > 0 && number <= Examples.Count)
            {
                var example = Examples[number - 1];
                Console.WriteLine($" {number}. {example.Description}");
                await example.Action(context);
            }
            else
            {
                Console.WriteLine($"no example with number ${number + 1}");
            }
        }

        private static void AddExample(string description, Func<Container, Task> action)
        {
            Examples.Add(new Example { Description = description, Action = action });
        }

        private static async Task<Contact> GetCurrentContact(Container ctx)
        {
            var userRequest = ctx.contact.getauthenticated();
            var users = await userRequest.ExecuteAsync();
            return users.First();
        }
    }
}
