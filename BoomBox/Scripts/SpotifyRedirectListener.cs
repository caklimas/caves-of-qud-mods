
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BoomBox.Scripts
{
    internal static class SpotifyRedirectListener
    {
        public static string StartHttpServerAsync(string redirectUri, string clientId, string clientSecret)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            Console.WriteLine("Listening for authorization code on " + redirectUri);

            var context = listener.GetContext();
            var code = context.Request.QueryString["code"];
            var response = context.Response;
            string responseString = "<html><body>You can close this window now.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();

            Console.WriteLine($"Authorization code received: {code}");

            // Exchange the authorization code for an access token
            return GetAccessToken(code, clientId, clientSecret, redirectUri);
        }

        private static string GetAccessToken(string code, string clientId, string clientSecret, string redirectUri)
        {
            var postData = $"grant_type=authorization_code&code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}&client_id={clientId}&client_secret={clientSecret}";
            var request = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";

            // Write the request data to the request stream
            var dataBytes = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = dataBytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(dataBytes, 0, dataBytes.Length);
            }

            // Send the request and get the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var pattern = "\"access_token\":\"([^\"]+)\"";
                        var responseText = reader.ReadToEnd();
                        var match = Regex.Match(responseText, pattern);

                        if (match.Success)
                        {
                            var accessToken = match.Groups[1].Value;
                            Console.WriteLine($"Access Token: {accessToken}");
                            return accessToken;
                        }
                        else
                        {
                            throw new Exception("Access token not found.");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Failed to get token. Status code: {response.StatusCode}");
                }
            }
        }
    }
}
