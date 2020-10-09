using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;

namespace InLooxApiExamples.OAuth
{
    class WebSignIn
    {
        // Hack to bring the Console window to front.
        // ref: http://stackoverflow.com/a/12066376
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        public string Token { get; private set; }

        public void OpenBrowserAndWait(string url)
        {
            // create a redirect URI using an available port on the loopback address.
            string redirectUri = "http://127.0.0.1:7890/";
            Console.WriteLine("redirect URI: " + redirectUri);

            // create an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            Console.WriteLine("Listening..");
            http.Start();

            var uriBuilder = new UriBuilder(url + "oauth2/authorize");
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["client_id"] = "F1CE1597340F423E90F52FAE6438F0E4";
            query["redirect_uri"] = url + "tests/oauthsignin";
            query["state"] = "my-nonce";
            query["scope"] = "Projects Plannings";
            query["response_type"] = "token";
            uriBuilder.Query = query.ToString() ?? "";
            var fullUrl = uriBuilder.ToString();


            var browser = new Browser();
            browser.OpenUrl(fullUrl);
            browser.Show();
            Token = browser.WaitForToken();
        }
        
        public static void BringConsoleToFront()
        {
            SetForegroundWindow(GetConsoleWindow());
        }
    }
}