using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace RestExample_OAuth
{
    public partial class OAuthDialog : Form
    {
        public String abortReason;
        private String oAuthCode;

        public OAuthDialog(Uri url)
        {
            Console.WriteLine("OAuthDialog");
            InitializeComponent();
            webBrowser = getBrowser();
            webBrowser.Url = url;
        }

        private void documentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            // Get the redirect url
            // Rally will pass this back in the form:
            // http://localhost:4567/oauth-redirect?code=cc482b7cee6a4d5a9ae989b3064cdeca&state=aa7df71b-27c4-4984-a7b3-4749a5840507
            // the code param is what we need
            string redirectUrl = e.Url.ToString();
            Console.WriteLine("Re-direct URL:");
            Console.WriteLine(redirectUrl);

            Uri redirectUri = new Uri(redirectUrl);
            string codeParam = HttpUtility.ParseQueryString(redirectUri.Query).Get("code");
            this.oAuthCode = codeParam;

            //Console.WriteLine(webBrowser.DocumentText);
            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();
            // We have been passed the OAuth Re-direct page, including our code parameter

        }

        private void navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            // Set text while the page has not yet loaded.
            this.Text = "Navigating";
        }

        public WebBrowser getBrowser()
        {
            return webBrowser;
        }

        public string getOAuthCode()
        {
            return this.oAuthCode;
        }
    }
}
