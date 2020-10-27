using InLoox.ODataClient;
using InLoox.ODataClient.Services;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using InLoox.ODataClient.Extensions;

namespace InLooxApiTests.Documents
{
    [TestClass]
    public class DocumentSyncServiceTest : TestBase
    {
        [TestMethod]
        public async Task MoveDocument_ToNewFolder_ShouldUpdateDocumentSync()
        {
            var (documentId, project) = await this.UploadDocument($"logo{DateTime.Now.Ticks}.jpg");
            
            Context.MergeOption = Microsoft.OData.Client.MergeOption.NoTracking;

            var latestChange = await GetLastChange(Context);
            Assert.AreEqual(latestChange.PrimaryKey, documentId);

            var docService = new DocumentService(Context);
            var folder = await docService.CreateFolder("NewFolder", project.ProjectId);
            await Context.SaveChangesAsync();
            
            var result = await docService.MoveDocument(documentId, folder.DocumentFolderId);
            Assert.IsTrue(result, "MoveDocument failed");

            var latestChange2 = await GetLastChange(Context);

            Assert.AreEqual(latestChange.PrimaryKey, latestChange2.PrimaryKey);
            Assert.AreNotEqual(latestChange.UpdatedAt, latestChange2.UpdatedAt);
        }

        private static async Task<DocumentSync> GetLastChange(Default.Container context)
        {
            var docSyncService = new DocumentSyncService(context);
            var changes = await docSyncService.GetLatestChanges(0, 1);
            return changes.First();
        }
    }
}
