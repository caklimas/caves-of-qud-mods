
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace SpotifyPlayer
{
    internal static class SpotifyRedirectListener
    {
        public static string StartHttpServer(String redirectUri, String clientId, String clientSecret)
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

                var response = client.Send(request);
                var json = response.Content.ReadAsStringAsync().Result;
                var token = JsonObject.Parse(json)["access_token"].GetValue<string>();
                Console.WriteLine($"Access Token: {token}");

                // Now you can use the access token to interact with the Spotify Web API
                //await PausePlaybackAsync(token);

                return token;
            }
        }
    }
}
