using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Default;
using InLoox.ODataClient;
using InLoox.ODataClient.Services;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.OData.Client;

namespace InLooxApiExamples.Examples
{
    public class DocumentExample
    {
        private readonly Container _ctx;

        public DocumentExample(Container ctx)
        {
            _ctx = ctx;
        }

        public async Task AddExampleFileToProject()
        {
            var projects = new ProjectService(_ctx);
            var project = await projects.GetFirstOpenProjectByName();
            Console.WriteLine(project.Name);
            var documentService = new DocumentService(_ctx);

            var fileStream = File.OpenRead("./ExampleData/inloox_logo.jpg");
            var documentId = await documentService
                .UploadDocument("inloox_logo.jpg", fileStream, project.ProjectId);
            if (!documentId.HasValue)
                Console.WriteLine("error while uploading");

            await documentService.DeleteFile(documentId.Value);
        }

        internal async Task CreateFolderAndListContent()
        {
            var projects = new ProjectService(_ctx);
            var project = await projects.GetFirstOpenProjectByName();
            Console.WriteLine("Using Project: " + project.Name);

            var documentService = new DocumentService(_ctx);
            var folder = await documentService.CreateFolder("testfolder", project.ProjectId);

            Console.WriteLine("List Folders");
            var folders = await documentService.GetFolders(project.ProjectId);
            foreach (var f in folders)
            {
                Console.WriteLine($"Id:{f.DocumentFolderId} FolderName:{f.FolderName}");
            }

            var entries = (await documentService.GetDocumentEntries(project.ProjectId))
                .ToList();
            PrintDocumentEntries(entries);

            await documentService.DeleteFolder(folder.DocumentFolderId);

            var doc = entries.FirstOrDefault(k => !k.IsFolder);

            Console.WriteLine($"downloading {doc.Name}");
            var resp = await documentService.DownloadDocument(doc.PrimaryKey);
            await using var file = File.OpenWrite(doc.Name);
            await resp.Content.CopyToAsync(file);
        }

        private static void PrintDocumentEntries(IEnumerable<DocumentEntry> entries)
        {
            Console.WriteLine("List Entries");
            foreach (var e in entries)
            {
                Console.WriteLine($"Id:{e.PrimaryKey} Name:{e.Name} IsFolder:{e.IsFolder}");
            }
        }

        public async Task GetLatestChanges()
        {
            var projects = new ProjectService(_ctx);
            var project = await projects.GetFirstOpenProjectByName();
            Console.WriteLine("Using Project: " + project.Name);
            var documentService = new DocumentService(_ctx);

            var entries = await documentService.GetLatestChanges(0, 100);
            PrintDocumentEntries(entries);
        }

        public async Task UpdateDocumentCustomField()
        {
            // lookup custom field "DocTest" id
            var ceDefaults = (await _ctx.customexpanddefaultextend.ExecuteAsync())
                .ToList();
            var cedDocument = ceDefaults.FirstOrDefault(k => k.DisplayName == "DocTest");

            if (cedDocument == null)
            {
                Console.WriteLine("Custom field 'DocTest' not found");
                return;
            }

            // build query for all documents with custom field 'DocTest' set to true.
            var query = _ctx.documentview
                .Where(k => k.CustomExpand.Any(ce => ce.CustomExpandDefaultId == cedDocument.CustomExpandDefaultId && ce.BoolValue == true))
                .OrderBy(k => k.FileName)
                .Skip(0)
                .Take(10);

            // need to use DataServiceCollection to use the PostOnlySetProperties feature
            // if you only need to read from the query a ToList() is ok.
            var docs = await ODataBasics.GetDSCollection(query);

            Console.WriteLine($"found {docs.Count} document{(docs.Count > 1 ? "s" : string.Empty)}:");

            foreach (var d in docs)
                Console.WriteLine(d.FileName);

            // change document state
            if (docs.Count > 0)
            {
                var doc = docs.First();
                doc.State = "Updated " + DateTime.Now;

                // only update the modfied properties
                _ctx.SaveChangesDefaultOptions = SaveChangesOptions.PostOnlySetProperties;

                _ctx.UpdateObject(doc);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
