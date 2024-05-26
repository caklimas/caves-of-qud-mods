using BoomBox.Scripts.Models;
using LitJson;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace BoomBox.Scripts.Spotify
{
    internal static class SpotifyClient
    {
        private const string BASE_URL = "https://api.spotify.com/v1/";

        public static void PausePlayback(String deviceId) {
            try
            {
                Console.WriteLine("Pausing spotify player");
                var baseUri = $"{BASE_URL}me/player/pause";

                var builder = new UriBuilder(baseUri);

                if (deviceId != null)
                {
                    Console.WriteLine("Device not null");
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["device_id"] = deviceId;
                    builder.Query = query.ToString();
                }

                Console.WriteLine($"Resume url: {builder.ToString()}");

                var request = getRequest(builder.ToString(), "PUT");
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
                        Console.WriteLine($" Pause Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static void ResumePlayback(string deviceId)
        {
            try
            {
                Console.WriteLine("Resuming spotify player");
                var baseUri = $"{BASE_URL}me/player/play";
                
                var builder = new UriBuilder(baseUri);

                if (deviceId != null)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["device_id"] = deviceId;
                    builder.Query = query.ToString();
                }

                Console.WriteLine($"Resume url: {builder.ToString()}");
                var request = getRequest(builder.ToString(), "PUT");
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
                        Console.WriteLine($"Resume Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static PlaybackStateResponse GetPlaybackState()
        {
            try
            {
                Console.WriteLine("Getting playback state");
                
                var request = getRequest($"{BASE_URL}me/player", "GET");
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

                            var responsePlayback = JsonMapper.ToObject<PlaybackStateResponse>(responseBody);
                            return responsePlayback;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    else
                    { 
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public static AvailableDevicesResponse GetAvailableDevices()
        {
            try
            {
                Console.WriteLine("Getting available devices");

                var request = getRequest($"{BASE_URL}me/player/devices", "GET");

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
                            Console.WriteLine($"Playback state: {responseBody}");

                            var responsePlayback = JsonMapper.ToObject<AvailableDevicesResponse>(responseBody);
                            return responsePlayback;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public static void TransferPlayback(string deviceId)
        {
            try
            {
                Console.WriteLine($"Transferring device to id {deviceId}");
                var request = getRequest($"{BASE_URL}me/player", "PUT");
                request.ContentType = "application/json";

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    var json = JsonMapper.ToJson(new TransferDevice() { device_ids = new[] { deviceId } });
                    streamWriter.Write(json);
                }

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

                            Console.WriteLine("Transfer Playback successful.");
                            Console.WriteLine($"{responseBody}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Transfer Playback Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Transfer Playback Exception: {ex.Message}");
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
