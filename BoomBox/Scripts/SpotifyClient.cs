using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoomBox.Scripts
{
    internal static class SpotifyClient
    {
        public static string GetCurrentUser(string accessToken)
        {
            try
            {
                // Create a HttpWebRequest for the specified URL
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.spotify.com/v1/me");
                request.Method = "GET";
                request.Headers["Authorization"] = "Bearer " + accessToken;

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
            catch (WebException ex)
            {
                // Handle any errors
                return $"WebException: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
