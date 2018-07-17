using InLooxOData;
using IQmedialab.InLoox.Data.BusinessObjects;
using Microsoft.OData.Client;
using System;
using System.Linq;

namespace InLooxnowClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var endPoint = new Uri("https://app.inlooxnow.com/");
            var endPointOdata = new Uri(endPoint, "odata/");

            var username = "user@inloox.com";
            var password = "";

            var tokenResponse = ODataBasics.GetToken(endPoint, username, password).Result;

            // in case multiple accounts exists
            if (tokenResponse.Error != null && tokenResponse.Error != "invalid_grant")
            {
                var accounts = tokenResponse.GetAccounts();

                // filter correct account by name
                var myAccount = accounts.FirstOrDefault(k => k.Name.StartsWith("000000"));
                tokenResponse = ODataBasics.GetToken(endPoint, username, password, myAccount.Id).Result;
            }

            if (tokenResponse?.AccessToken == null)
            {
                Console.WriteLine("Login invalid");
                return;
            }

            var context = ODataBasics.GetInLooxContext(endPointOdata, tokenResponse.AccessToken);
            // lookup custom field "DocTest" id
            var ceDefaults = context.customexpanddefaultextend.ToList();
            var cedDocument = ceDefaults.FirstOrDefault(k => k.DisplayName == "DocTest");

            if (cedDocument == null)
            {
                Console.WriteLine("Custom field 'DocTest' not found");
                return;
            }

            // build query for all documents with custom field 'DocTest' set to true.
            var query = context.documentview
                .Where(k => k.CustomExpand.Any(ce => ce.CustomExpandDefaultId == cedDocument.CustomExpandDefaultId && ce.BoolValue == true))
                .OrderBy(k => k.FileName)
                .Skip(0)
                .Take(10);

            // need to use DataServiceCollection to use the PostOnlySetProperties feature
            // if you only need to read from the query a ToList() is ok.
            var docs = new DataServiceCollection<DocumentView>(query);

            Console.WriteLine($"found {docs.Count()} document{(docs.Count() > 1 ? "s" : String.Empty)}:");

            foreach (var d in docs)
                Console.WriteLine(d.FileName);

            // change document state
            if (docs.Count > 0)
            {
                var doc = docs.FirstOrDefault();
                doc.State = "Updated " + DateTime.Now;

                // only update the modfied properties
                context.SaveChangesDefaultOptions = SaveChangesOptions.PostOnlySetProperties;

                context.UpdateObject(doc);
                context.SaveChanges();
            }
        }
    }
}
