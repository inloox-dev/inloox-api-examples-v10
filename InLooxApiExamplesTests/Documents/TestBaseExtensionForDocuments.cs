using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InLooxApiTests.Documents
{
    static class TestBaseExtensionForDocuments
    {
        public static async Task<(Guid, ProjectView)> UploadDocumentToFirstProject(this TestBase testBase, string name)
        {
            var projectService = new ProjectService(testBase.Context);
            var project = await projectService.GetFirstOpenProjectByName();

            var docService = new DocumentService(testBase.Context);
            using var f = File.OpenRead("./ExampleData/inloox_logo.jpg");
            var documentId = await docService.UploadDocument(name, f, project.ProjectId);
            return (documentId.Value, project);
        }
    }
}

