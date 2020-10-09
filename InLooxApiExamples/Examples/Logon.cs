using System;
using System.Linq;
using System.Threading;
using InLoox.ODataClient;
using InLooxApiExamples.OAuth;

namespace InLooxApiExamples.Examples
{
    public class Logon
    {
        public static string LogonOauth(Uri endPoint)
        {
            var webSignIn = new WebSignIn();

            var uiThread = new Thread(() => webSignIn.OpenBrowserAndWait(endPoint.ToString()));
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            uiThread.Join();

            WebSignIn.BringConsoleToFront();
            return webSignIn.Token;
        }

        public static ODataBasics.TokenResponse LogonWithCredentials(Uri endPointOdata, string username, string password)
        {
            var tokenResponse = ODataBasics.GetToken(endPointOdata, username, password)
                .Result;
            return tokenResponse;
        }


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
