using Default;
using InLoox.ODataClient;
using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InLooxnowClient.Examples
{
    public class DocumentQueries
    {
        private readonly Container _ctx;

        public DocumentQueries(Container ctx)
        {
            _ctx = ctx;
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
                var doc = docs.FirstOrDefault();
                doc.State = "Updated " + DateTime.Now;

                // only update the modfied properties
                _ctx.SaveChangesDefaultOptions = SaveChangesOptions.PostOnlySetProperties;

                _ctx.UpdateObject(doc);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
