using System;
using System.IO;
using System.Net;
using System.Text;

namespace BoomBox.Scripts
{
    internal static class SpotifyClient
    {
        private const string BASE_URL = "https://api.spotify.com/v1/";

        public static void PausePlayback() {
            try
            {
                Console.WriteLine("Pausing spotify player");

                var request = getRequest($"{BASE_URL}me/player/pause", "PUT");
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

        public static void ResumePlayback()
        {
            try
            {
                Console.WriteLine("Resuming spotify player");

                var request = getRequest($"{BASE_URL}me/player/play", "PUT");
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

                            Console.WriteLine("Playback resumed successfully.");
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

        private static HttpWebRequest getRequest(string url, string method)
        {
            Console.WriteLine($"Creating request for {url}");

            // Create a HttpWebRequest for the specified URL
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Headers["Authorization"] = "Bearer " + SpotifyLoader.token;

            return request;
        }
    }
}
