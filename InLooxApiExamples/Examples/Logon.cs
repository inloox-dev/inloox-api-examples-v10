using InLoox.ODataClient;
using System;
using System.Linq;

namespace InLooxnowClient.Examples
{
    public class Logon
    {
        public static ODataBasics.TokenResponse LogonMultipleAccounts(Uri endPoint,
            string username, string password, string accountName)
        {
            var tokenResponse = ODataBasics.GetToken(endPoint, username, password).Result;
            if (tokenResponse.Error != null && tokenResponse.Error != "invalid_grant")
            {
                var accounts = tokenResponse.GetAccounts();

                // filter correct account by name
                var myAccount = accounts.FirstOrDefault(k => k.Name.StartsWith(accountName));
                tokenResponse = ODataBasics.GetToken(endPoint, username, password, myAccount.Id)
                    .Result;
            }

            return tokenResponse;
        }
    }
}
