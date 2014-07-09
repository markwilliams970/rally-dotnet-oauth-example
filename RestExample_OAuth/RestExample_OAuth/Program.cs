using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;
using System.Diagnostics;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace RestExample_OAuth
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            string oAuthServer = "https://rally1.rallydev.com/login/oauth2";
            string rallyAuthUrl = oAuthServer + "/auth";
            string rallyTokenUrl = oAuthServer + "/token";
            string redirectUrl = "http://localhost:4567/done";

            string clientID = "725b7550782e4a59bef8f6176950322e";
            string clientSecret = "r3f87iogm47dhf4gb0wosh7zkpgjmnztq4rz27npvx";

            string callBackUrl = "http://localhost:4567/done";
            // callback HTML page on localhost

            // OAuth params
            NameValueCollection oAuthParams = new NameValueCollection();
            oAuthParams["state"] = System.Guid.NewGuid().ToString();
            oAuthParams["response_type"] = "code";
            oAuthParams["redirect_uri"] = callBackUrl;
            oAuthParams["client_id"] = clientID;
            oAuthParams["scope"] = "openid";

            // OAuth URI params
            string uriParams = toQueryString(oAuthParams);

            // OAuth URL
            string oAuthUrl = rallyAuthUrl + uriParams;

            // OAuth URI
            Uri oAuthUri = new Uri(oAuthUrl);

            Console.WriteLine("Press any Key to Instantiate OAuth Dialog...");
            Console.ReadKey();
            OAuthDialog form = new OAuthDialog(oAuthUri);
            DialogResult result = form.ShowDialog();

            // Get the OAuth Code provided by Rally's response
            string code = form.getOAuthCode();

            Console.WriteLine("OAuth code:");
            Console.WriteLine(code);

            Console.WriteLine("Press Any Key to Continue...");
            Console.ReadKey();

            Console.WriteLine("Getting OAuth Token from Rally...");

            // Now finally, go get token
            var client = new RestClient(rallyTokenUrl);

            var tokenRequest = new RestRequest(Method.POST);
            
            // add Uri Params
            tokenRequest.AddParameter("code", code); // adds to POST or URL querystring based on Method
            tokenRequest.AddParameter("redirect_uri", redirectUrl);
            tokenRequest.AddParameter("grant_type", "authorization_code");
            tokenRequest.AddParameter("client_id", clientID);
            tokenRequest.AddParameter("client_secret", clientSecret);

            //  add HTTP Headers
            tokenRequest.AddHeader("content_type", "application/x-www-form-urlencode");
            tokenRequest.AddHeader("accept", "json");

            // execute the request
            RestResponse tokenResponse = (RestResponse) client.Execute(tokenRequest);

            // Obtain the oAuthToken from the Response
            var content = tokenResponse.Content;
            Console.WriteLine("content: ");
            Console.WriteLine(content);

            // De-serialize
            var jsonContent = @content;
            dynamic jsonObject = JValue.Parse(jsonContent);
            string oAuthToken = jsonObject.access_token;

            Console.WriteLine("OAuth Access token:");
            Console.WriteLine(oAuthToken);
            Console.WriteLine("Press Any key to Continue...");
            Console.ReadKey();

            // Now, finally! Let's go get some stories from Rally
            Console.WriteLine("Querying Rally for Stories...");
            var rallyClient = new RestClient("https://rally1.rallydev.com/slm/webservice/v2.0/hierarchicalrequirement");
            var storiesRequest = new RestRequest(Method.GET);

            // add Uri Params
            storiesRequest.AddParameter("query", "(FormattedID < 20)"); // adds to POST or URL querystring based on Method
            storiesRequest.AddParameter("fetch", "Name,FormattedID");

            //  add HTTP Headers
            storiesRequest.AddHeader("ZSESSIONID", oAuthToken);
            storiesRequest.AddHeader("accept", "json");

            // execute the request
            RestResponse storiesResponse = (RestResponse)rallyClient.Execute(storiesRequest);

            // Obtain the results json from the Response
            var storiesContent = storiesResponse.Content;
            Console.WriteLine("Stories raw content: ");
            Console.WriteLine(storiesContent);

            Console.ReadKey();

        }

        static string toQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                         from value in nvc.GetValues(key)
                         select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
                .ToArray();
            return "?" + string.Join("&", array);
        } 
    }
}