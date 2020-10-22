using Default;
using InLoox.ODataClient.Services;
using IQmedialab.InLoox.Data.Api.Model.OData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InLooxApiExamples.Examples
{
    public class DocumentSyncExample
    {
        private readonly Container _ctx;

        public DocumentSyncExample(Container ctx)
        {
            _ctx = ctx;
        }
        
        private static void PrintDocumentEntries(IEnumerable<DocumentSync> entries)
        {
            Console.WriteLine("List Entries");
            foreach (var e in entries)
            {
                Console.WriteLine($"Id:{e.PrimaryKey} Name:{e.Name} IsFolder:{e.IsFolder} Deleted: {e.Deleted}");
            }
        }

        public async Task GetLatestChanges()
        {
            var projects = new ProjectService(_ctx);
            var project = await projects.GetFirstOpenProjectByName();
            Console.WriteLine("Using Project: " + project.Name);
            var documentSyncService = new DocumentSyncService(_ctx);

            var entries = await documentSyncService.GetLatestChanges(0, 100);
            PrintDocumentEntries(entries);
        }
    }
}
