using Default;
using InLooxOData;
using IQmedialab.InLoox.Data.BusinessObjects;
using Microsoft.OData.Client;
using System;
using System.Linq;

namespace InLooxnowClient.Examples
{
    public class ContactQueries
    {
        private readonly Container _ctx;

        public ContactQueries(Container ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// creates a inloox user, adds default (empty) permission set and
        /// sets the user's password
        /// </summary>
        /// <param name="displayName">contact display name</param>
        /// <param name="email">unique email address</param>
        /// <param name="password">user password, minimum 8 letters, upper- and lowercase & number</param>
        /// <returns></returns>
        public Contact CreateUserAndAuth(string displayName, string email, string password)
        {
            var c = CreateContact(displayName, email);
            AddPermission(c);
            SetPassword(c, password);
            return c;
        }

        public Contact CreateContact(string displayName, string email)
        {
            var dsContacts = ODataBasics.GetDSCollection<Contact>(_ctx);

            // first add then change properties
            var contact = new Contact();
            dsContacts.Add(contact);
            contact.DisplayName = displayName;
            contact.Email = email;

            _ctx.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            return contact;
        }

        public void AddPermission(Contact c)
        {
            _ctx.SendingRequest2 += _ctx_SendingRequest2;
            try
            {
                _ctx.userpermissionextend.addbycontactid(c.ContactId).GetValue();
            }
            catch (DataServiceQueryException ex)
            {
                // action is successfull but odata client cant parse response
                Console.WriteLine(ex.Message);
            }
            _ctx.SendingRequest2 -= _ctx_SendingRequest2;
        }

        private void _ctx_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            e.RequestMessage.SetHeader("Accept", "*/*");
            e.RequestMessage.SetHeader("X-Requested-With", "XMLHttpRequest");
        }

        public void SetPassword(Contact c, string password)
        {
            var query = _ctx.authentication
                .Where(k => k.ContactId == c.ContactId);
            var dsAuth = ODataBasics.GetDSCollection(query);
            var auth = dsAuth.First();
            auth.Password = password;
            _ctx.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
        }
    }
}
