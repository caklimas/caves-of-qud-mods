using System;
using System.IO;
using System.Net;
using System.Text;

namespace BoomBox.Scripts
{
    internal static class SpotifyClient
    {
        private const string BASE_URL = "https://api.spotify.com/v1/";

        public static string GetCurrentUser(string accessToken)
        {
            try
            {
                var request = getRequest($"{BASE_URL}me", "GET", accessToken);   
                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check the response status code
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Read the response stream
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseBody = reader.ReadToEnd();

                            // Get the display name of the user
                            return $"Current user: {responseBody}";
                        }
                    }
                    else
                    {
                        return $"Error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        public static void PausePlayback(string accessToken) {
            try
            {
                Console.WriteLine("Pausing spotify player");

                var request = getRequest($"{BASE_URL}me/player/pause", "PUT", accessToken);
                request.ContentLength = 0;

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check the response status code
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Read the response stream
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseBody = reader.ReadToEnd();
                            
                            Console.WriteLine("Playback paused successfully.");
                            Console.WriteLine($"{responseBody}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        private static HttpWebRequest getRequest(string url, string method, string accessToken)
        {
            Console.WriteLine($"Creating request for {url}");

            // Create a HttpWebRequest for the specified URL
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Headers["Authorization"] = "Bearer " + accessToken;

            return request;
        }
    }
}
