using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Services;
using Microsoft.OData.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InLooxApiTests.Documents
{
    [TestClass]
    public class DocumentServiceTest : TestBase
    {
        [TestMethod]
        public async Task RenameDocument_WithNewName_ShouldChangeFileName()
        {
            var oldName = "logo.jpg";
            var newName = "logo_1.jpg";

            var (documentId, project) = await this.UploadDocument(oldName);

            var docService = new DocumentService(Context);
            await docService.RenameDocument(documentId, newName);

            var files = await docService.GetFiles(project.ProjectId);
            var file = files.First(k => k.DocumentId == documentId);

            Assert.AreEqual(newName, file.FileName);
        }

        [TestMethod]
        public async Task UpdateIsHidden_SetTrue_ShouldChangeProperty()
        {
            await UpdateDocumentField(nameof(DocumentView.IsHidden),
                (d) => d.IsHidden = true,
                (d) => d.IsHidden,
                true);
        }

        [TestMethod]
        public async Task UpdateCreatedDate_SetToNow_ShouldChangeProperty()
        {
            var date = DateTime.Now;

            await UpdateDocumentField(nameof(DocumentView.CreatedDate),
                (d) => d.CreatedDate = date,
                (d) => d.CreatedDate,
                date);
        }

        [TestMethod]
        public async Task UpdateChangedDate_SetToNow_ShouldChangeProperty()
        {
            var date = DateTime.Now;

            await UpdateDocumentField(nameof(DocumentView.ChangedDate),
                (d) => d.ChangedDate = date,
                (d) => d.ChangedDate,
                date);
        }

        private async Task UpdateDocumentField(string propName,
            Action<DocumentView> setterFunc, Func<DocumentView, object> getterFunc, object newVal)
        {
            var (documentId, _) = await this.UploadDocument("logo.jpg");
            var file = await GetDocumentFromCollection(documentId);

            setterFunc(file);
            await Context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            file = await GetDocumentFromCollection(documentId);
            Assert.AreEqual(newVal, getterFunc(file), $"Couldnt set {propName}");
        }

        private async Task<DocumentView> GetDocumentFromCollection(Guid documentId)
        {
            var collection = await ODataBasics.GetDSCollection(
                Context.documentview.Where(k => k.DocumentId == documentId)
            );
            return collection.First();
        }
    }
}
