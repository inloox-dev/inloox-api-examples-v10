using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Services;
using Microsoft.OData.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
        public async Task ReplaceDocument_ImageFile_ShouldReplaceTheOldFile()
        {
            var fileOriginal = await UploadFile();
            var logoNoTextRaw = File.ReadAllBytes("./ExampleData/inloox_logo_no_text.png");

            using var memoryStream = new MemoryStream(logoNoTextRaw);

            var docService = new DocumentService(Context);
            var success = await docService.ReplaceDocument(fileOriginal.DocumentId, memoryStream);
            Assert.IsTrue(success, "replace document unsuccessfull");

            var resp = await docService.DownloadDocument(fileOriginal.DocumentId);
            var logoDownloadedRaw = await resp.Content.ReadAsByteArrayAsync();

            CollectionAssert.AreEqual(logoNoTextRaw, logoDownloadedRaw, "content is not the same");

            // filesize is not updated
            //var fileReplaced = await docService.GetDocumentFromCollection(fileOriginal.DocumentId);
            //Assert.AreNotEqual(fileOriginal.FileSize, fileReplaced.FileSize);
        }

        [TestMethod]
        public async Task UpdateIsHidden_SetTrue_ShouldChangeProperty()
        {
            await UpdateDocumentField(nameof(DocumentView.IsHidden),
                d => d.IsHidden = true,
                d => d.IsHidden,
                true);
        }

        [TestMethod]
        public async Task UpdateFileCreatedDate_FixedDate_ShouldChangeProperty()
        {
            var date = new DateTimeOffset(2020, 10, 28, 10, 0, 0, TimeSpan.FromHours(1));

            await UpdateDocumentField(nameof(DocumentView.FileCreatedDate),
                d => d.FileCreatedDate = date,
                d => d.FileCreatedDate.Value,
                date);
        }

        [TestMethod]
        public async Task UpdateFileChangedDate_FixedDate_ShouldChangeProperty()
        {
            var date = new DateTimeOffset(2020, 10, 28, 10, 0, 0, TimeSpan.FromHours(1));

            await UpdateDocumentField(nameof(DocumentView.FileChangedDate),
                d => d.FileChangedDate = date,
                d => d.FileChangedDate.Value,
                date);
        }

        private async Task UpdateDocumentField(string propName,
            Action<DocumentView> setterFunc, Func<DocumentView, object> getterFunc, object newVal)
        {
            var (documentId, _) = await this.UploadDocumentToFirstProject("logo.jpg");
            var file = await GetDocument(documentId);

            setterFunc(file);
            await Context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            file = await GetDocument(documentId);
            Assert.AreEqual(newVal, getterFunc(file), $"Couldnt set {propName}");
        }

        private async Task<DocumentView> UploadFile()
        {
            var (documentId, _) = await this.UploadDocumentToFirstProject($"logo_{DateTime.Now.Ticks}.jpg");
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
