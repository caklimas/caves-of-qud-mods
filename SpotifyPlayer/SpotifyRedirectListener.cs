using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyPlayer
{
    internal static class SpotifyRedirectListener
    {
        public static async Task<string> StartHttpServerAsync(String redirectUri, String clientId, String clientSecret)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            Console.WriteLine("Listening for authorization code on " + redirectUri);

            var context = await listener.GetContextAsync();
            var code = context.Request.QueryString["code"];
            var response = context.Response;
            string responseString = "<html><body>You can close this window now.</body></html>";
            var buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();

            Console.WriteLine($"Authorization code received: {code}");

            // Exchange the authorization code for an access token
            return await GetAccessTokenAsync(code, clientId, clientSecret, redirectUri);
        }

        private static async Task<string> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
                request.Content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

                var response = await client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var token = JObject.Parse(json).GetValue("access_token").ToString();
                Console.WriteLine($"Access Token: {token}");

                // Now you can use the access token to interact with the Spotify Web API
                //await PausePlaybackAsync(token);

                return token;
            }
        }
    }
}
