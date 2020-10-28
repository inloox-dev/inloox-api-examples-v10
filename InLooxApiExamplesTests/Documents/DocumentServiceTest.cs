using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Services;
using Microsoft.OData.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace InLooxApiTests.Documents
{
    [TestClass]
    public class DocumentServiceTest : TestBase
    {

        [TestMethod]
        public async Task RenameDocument_WithNewName_ShouldChangeFileName()
        {
            var newName = "logo_1.jpg";

            var file = await UploadFile();

            var docService = new DocumentService(Context);
            await docService.RenameDocument(file.DocumentId, newName);

            file = await GetDocument(file.DocumentId);

            Assert.AreEqual(newName, file.FileName);
        }

        [TestMethod]
        public async Task UpdateIsHidden_SetTrue_ShouldChangeProperty()
        {
            var file = await UploadFile();
            file.IsHidden = true;
            await Context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            file = await GetDocument(file.DocumentId);
            Assert.AreEqual(true, file.IsHidden, "setting IsHidden failed");
        }

        [TestMethod]
        public async Task UpdateCreatedDate_SetToNow_ShouldChangeProperty()
        {
            var date = DateTimeOffset.Now;

            var file = await UploadFile();
            file.CreatedDate = date;
            await Context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            file = await GetDocument(file.DocumentId);
            AssertDateTimeOffset(date, file.CreatedDate);
        }

        [TestMethod]
        public async Task UpdateChangedDate_SetToNow_ShouldChangeProperty()
        {
            var date = DateTimeOffset.Now;

            var file = await UploadFile();
            file.ChangedDate = date;
            await Context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            file = await GetDocument(file.DocumentId);
            AssertDateTimeOffset(date, file.ChangedDate.Value);
        }

        private void AssertDateTimeOffset(DateTimeOffset d1, DateTimeOffset d2)
        {
            Assert.IsTrue(TimeSpan.FromSeconds(1) > d2 - d1,
                $"DateTime differ {d2 - d1} which is more than a second: {d1}, {d2}");
        }

        private async Task<DocumentView> UploadFile()
        {
            var (documentId, _) = await this.UploadDocumentToFirstProject("logo.jpg");
            var file = await GetDocument(documentId);

            return file;
        }

        private Task<DocumentView> GetDocument(Guid documentId)
        {
            var docService = new DocumentService(Context);
            return docService.GetDocumentFromCollection(documentId);
        }
    }
}
