using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace InLooxApiExamples.OAuth
{
    public partial class Browser : Form
    {
        private WebBrowser _browser;
        private string _token = string.Empty;

        public Browser()
        {
            InitializeComponent();

            _browser = new WebBrowser();
            _browser.Dock = DockStyle.Fill;
            _browser.Navigated += _browser_Navigated;

            this.Controls.Add(_browser);
        }

        private void _browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.Fragment.StartsWith("#access_token"))
            {
                _token = HttpUtility.ParseQueryString(e.Url.Fragment.Substring(1)).Get("access_token");
            }
        }

        public void OpenUrl(string url)
        {
            _browser.Navigate(url);
        }

        public string WaitForToken()
        {
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(_token))
                {
                    return _token;
                }
                Thread.Sleep(100);
                Application.DoEvents();
            }
        }
    }
}
