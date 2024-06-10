
using Qudify.Scripts.Models;
using LitJson;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Qudify.Scripts.Spotify
{
    internal static class SpotifyRedirectListener
    {
        internal static SpotifyAccessToken StartHttpServerAsync(string redirectUri, string clientId, string codeVerifier)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            Console.WriteLine("Listening for authorization code on " + redirectUri);

            new Thread(() =>
            {
                Thread.Sleep(10_000);
                Console.WriteLine("Stopping listener");
                listener.Stop();
            }).Start();

            try
            {
                var context = listener.GetContext();
                var code = context.Request.QueryString["code"];
                var response = context.Response;
                string responseString = "<html><body>Logged into Spotify, you can now close the window and go back to Caves of Qud.<br />Live and drink friend!</body></html>";
                var buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
                listener.Stop();

                Console.WriteLine($"Authorization code received: {code}");

                // Exchange the authorization code for an access token
                return GetAccessToken(code, clientId, codeVerifier, redirectUri);
            }
            catch (HttpListenerException)
            {
                return null;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static SpotifyAccessToken GetAccessToken(string code, string clientId, string codeVerifier, string redirectUri)
        {
            var postData = $"grant_type=authorization_code&code={code}&redirect_uri={Uri.EscapeDataString(redirectUri)}&client_id={clientId}&code_verifier={codeVerifier}";
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
                        var responseText = reader.ReadToEnd();
                        Console.WriteLine($"Access Token Response: {responseText}");

                        var accessTokenResponse = JsonMapper.ToObject<AccessTokenResponse>(responseText);
                        return new SpotifyAccessToken { AccessToken = accessTokenResponse, Created = DateTime.UtcNow };
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
